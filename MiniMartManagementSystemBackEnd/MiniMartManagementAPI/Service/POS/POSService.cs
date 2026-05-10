using AutoMapper;
using BackEnd.Core.Domain.Entities;
using BackEnd.Core.Models.Payment;
using BackEnd.Core.Models.Request;
using BackEnd.Core.Models.Response;
using BackEnd.Core.SeedWorks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MiniMartManagementAPI.SinaglR;

namespace MiniMartManagementAPI.Service.POS
{
    public class POSService : IPOSService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IVNPayService _vnPayService;
        private readonly IHubContext<InventoryHub> _inventoryHub;
        private readonly ILogger<POSService> _logger;
        private const decimal AMOUNT_TOLERANCE = 0.01m;

        public POSService(IUnitOfWork unitOfWork, IMapper mapper, IVNPayService vnPayService, IHubContext<InventoryHub> inventoryHub, ILogger<POSService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _vnPayService = vnPayService;
            _inventoryHub = inventoryHub;
            _logger = logger;
        }

        private async Task<string> GenerateOrderCode()
        {
            var today = DateTime.Now.Date;
            int countToday = await _unitOfWork.OrderRepository.GetQueryable()
                .CountAsync(o => o.CreatedAt.Date == today);
            return $"HD{today:yyyyMMdd}{(countToday + 1):D4}";
        }

        public async Task<OrderResponseDTO> CreateOrderByCash(OrderRequestDTO request, string staffName)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var order = _mapper.Map<OrderEntity>(request);
                order.Id = Guid.NewGuid();
                order.OrderCode = await GenerateOrderCode();
                order.CreatedAt = DateTime.Now;
                order.Status = OrderStatus.Completed;

                var printItems = new List<object>();

                foreach (var itemRequest in request.Items)
                {
                    var product = await _unitOfWork.ProductRepository.GetByIdAsync(itemRequest.ProductId, trackChanges: true);
                    if (product == null) throw new Exception($"Sản phẩm {itemRequest.ProductId} không tồn tại.");
                    if (product.Quantity < itemRequest.Quantity) throw new Exception($"Sản phẩm '{product.Name}' không đủ tồn kho.");

                    product.Quantity -= itemRequest.Quantity;
                    await _unitOfWork.ProductRepository.UpdateAsync(product);

                    printItems.Add(new { ProductName = product.Name, Quantity = itemRequest.Quantity, Price = itemRequest.Price, SubTotal = itemRequest.Total });
                }

                _unitOfWork.OrderRepository.Add(order);
                await _unitOfWork.CompleteAsync();
                await transaction.CommitAsync();

                return _mapper.Map<OrderResponseDTO>(order);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<object> CreateOrderOnline(OrderRequestDTO request, string staffName, HttpContext httpContext)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var order = _mapper.Map<OrderEntity>(request);
                order.Id = Guid.NewGuid();
                order.OrderCode = await GenerateOrderCode();
                order.CreatedAt = DateTime.Now;
                order.Status = OrderStatus.Pending;
                order.PaymentMethod = "VNPay";

                foreach (var itemRequest in request.Items)
                {
                    var product = await _unitOfWork.ProductRepository.GetByIdAsync(itemRequest.ProductId);
                    if (product == null) throw new Exception($"Sản phẩm {itemRequest.ProductId} không tồn tại.");
                    if (product.Quantity < itemRequest.Quantity) throw new Exception($"Sản phẩm '{product.Name}' không đủ tồn kho.");
                }

                _unitOfWork.OrderRepository.Add(order);
                await _unitOfWork.CompleteAsync();
                await transaction.CommitAsync();

                var vnPayRequest = new VnPaymentRequestModel
                {
                    StoreId = order.StoreId,
                    OrderId = order.Id,
                    OrderCode = order.OrderCode,
                    FullName = staffName,
                    Description = $"Thanh toan don hang {order.OrderCode}",
                    Amount = (double)order.FinalAmount,
                    CreatedDate = order.CreatedAt,
                    ExpireDate = order.CreatedAt.AddMinutes(15).ToString("yyyyMMddHHmmss")
                };

                var paymentUrl = await _vnPayService.CreatePaymentUrl(httpContext, vnPayRequest);
                return new { OrderId = order.Id, OrderCode = order.OrderCode, PaymentUrl = paymentUrl };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<OrderResponseDTO>> GetAllOrders()
        {
            var orders = await _unitOfWork.OrderRepository.GetAllAsync();
            return _mapper.Map<List<OrderResponseDTO>>(orders);
        }

        public async Task<OrderResponseDTO> GetOrderById(Guid id)
        {
            var orderEntity = await _unitOfWork.OrderRepository.GetQueryable()
                .Include(o => o.Store).Include(o => o.Employee).Include(o => o.Items).ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
            return _mapper.Map<OrderResponseDTO>(orderEntity);
        }

        public async Task<IEnumerable<OrderResponseDTO>> GetOrdersByCode(string code)
        {
            var orders = _unitOfWork.OrderRepository.Find(o => o.OrderCode.Contains(code));
            return _mapper.Map<IEnumerable<OrderResponseDTO>>(orders);
        }

        public async Task<IEnumerable<OrderResponseDTO>> GetOrdersByDate(DateTime date)
        {
            var orders = _unitOfWork.OrderRepository.Find(o => o.CreatedAt.Date == date.Date);
            return _mapper.Map<IEnumerable<OrderResponseDTO>>(orders);
        }

        public async Task<bool> CancelOrder(Guid id)
        {
            var order = await _unitOfWork.OrderRepository.GetQueryable().Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == id);
            if (order == null || order.Status == OrderStatus.Cancelled) return false;

            order.Status = OrderStatus.Cancelled;
            foreach (var item in order.Items)
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(item.ProductId, trackChanges: true);
                if (product != null)
                {
                    product.Quantity += item.Quantity;
                    await _unitOfWork.ProductRepository.UpdateAsync(product);
                }
            }

            await _unitOfWork.CompleteAsync();
            await _inventoryHub.Clients.All.SendAsync("UpdateInventory", "Kho đã cập nhật");
            return true;
        }

        public async Task<object> ProcessPaymentCallback(IQueryCollection query)
        {
            var response = await _vnPayService.PaymentExecute(query);
            if (response == null || !response.Success) return new { RspCode = "97", Message = "Invalid signature" };

            if (!Guid.TryParse(response.OrderId, out var orderId)) return new { RspCode = "01", Message = "Order not found" };

            var order = await _unitOfWork.OrderRepository.GetQueryable().Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null) return new { RspCode = "01", Message = "Order not found" };

            if (order.Status == OrderStatus.Completed) return new { RspCode = "00", Message = "Confirm Success" };

            decimal amountDifference = Math.Abs((decimal)response.vnp_Amount - order.FinalAmount);
            if (amountDifference > AMOUNT_TOLERANCE) return new { RspCode = "04", Message = "Invalid amount" };

            if (order.Status != OrderStatus.Pending) return new { RspCode = "02", Message = "Order already confirmed" };

            if (response.VnPayResponseCode == "00" && response.vnp_TransactionStatus == "00")
            {
                order.Status = OrderStatus.Completed;
                foreach (var item in order.Items)
                {
                    var product = await _unitOfWork.ProductRepository.GetByIdAsync(item.ProductId, trackChanges: true);
                    if (product != null)
                    {
                        product.Quantity -= item.Quantity;
                        if (product.Quantity < 0) product.Quantity = 0;
                        await _unitOfWork.ProductRepository.UpdateAsync(product);
                    }
                }
                await _unitOfWork.OrderRepository.UpdateAsync(order);
                await _unitOfWork.CompleteAsync();
                await _inventoryHub.Clients.All.SendAsync("PaymentSuccess", new { OrderId = order.Id, OrderCode = order.OrderCode, Message = "Thanh toán thành công qua VNPay" });
                return new { RspCode = "00", Message = "Confirm Success" };
            }
            else
            {
                order.Status = OrderStatus.Cancelled;
                await _unitOfWork.OrderRepository.UpdateAsync(order);
                await _unitOfWork.CompleteAsync();
                await _inventoryHub.Clients.All.SendAsync("PaymentFailed", new { OrderId = order.Id, OrderCode = order.OrderCode, Message = $"Thanh toán thất bại. Mã lỗi VNPay: {response.VnPayResponseCode}" });
                return new { RspCode = "99", Message = "Payment processing failed" };
            }
        }

        public async Task<object> GetPaymentStatus(Guid orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (order == null) return null!;

            return new
            {
                OrderCode = order.OrderCode,
                Status = order.Status.ToString(),
                CanRetry = order.Status == OrderStatus.Cancelled || order.Status == OrderStatus.Pending,
                Message = order.Status switch
                {
                    OrderStatus.Completed => "Thanh toán thành công",
                    OrderStatus.Cancelled => "Thanh toán thất bại. Bạn có thể thử lại",
                    OrderStatus.Pending => "Chờ thanh toán. Vui lòng hoàn tất thanh toán",
                    _ => "Trạng thái không xác định"
                }
            };
        }

        public async Task<object> CreateRetryPaymentUrl(Guid orderId, HttpContext httpContext)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (order == null) return null!;

            if (order.Status != OrderStatus.Cancelled && order.Status != OrderStatus.Pending) return null!;

            if (order.Status == OrderStatus.Cancelled)
            {
                order.Status = OrderStatus.Pending;
                await _unitOfWork.OrderRepository.UpdateAsync(order);
                await _unitOfWork.CompleteAsync();
            }

            var vnPayRequest = new VnPaymentRequestModel
            {
                StoreId = order.StoreId,
                OrderId = order.Id,
                OrderCode = order.OrderCode,
                FullName = "N/V Bán Hàng",
                Description = $"Thanh toan lai don hang {order.OrderCode}",
                Amount = (double)order.FinalAmount,
                CreatedDate = DateTime.Now,
                ExpireDate = DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss")
            };

            var paymentUrl = await _vnPayService.CreatePaymentUrl(httpContext, vnPayRequest);
            return new { Message = "Tạo đơn hàng chờ thanh toán lại thành công", OrderCode = order.OrderCode, PaymentUrl = paymentUrl };
        }

        public async Task<int> CancelExpiredPendingOrders()
        {
            var fifteenMinutesAgo = DateTime.Now.AddMinutes(-15);
            var expiredOrders = await _unitOfWork.OrderRepository.GetQueryable()
                .Include(o => o.Items)
                .Where(o => o.Status == OrderStatus.Pending && o.CreatedAt < fifteenMinutesAgo)
                .ToListAsync();

            int cancelledCount = 0;
            foreach (var order in expiredOrders)
            {
                order.Status = OrderStatus.Cancelled;
                await _unitOfWork.OrderRepository.UpdateAsync(order);
                cancelledCount++;
            }

            if (cancelledCount > 0) await _unitOfWork.CompleteAsync();
            return cancelledCount;
        }
    }
}
