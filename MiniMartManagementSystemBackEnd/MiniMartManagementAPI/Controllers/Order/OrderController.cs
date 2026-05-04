using AutoMapper;
using BackEnd.Core.Domain.Entities;
using BackEnd.Core.Models;
using BackEnd.Core.Models.Request;
using BackEnd.Core.Models.Response;
using BackEnd.Core.Models.Function;
using BackEnd.Core.SeedWorks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MiniMartManagementAPI.Service;
using MiniMartManagementAPI.SinaglR;

namespace MiniMartManagementAPI.Controllers.Order
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "RootAdmin, Manager, Staff")]
    public class OrderController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHubContext<InventoryHub> _inventoryHub;

        public OrderController(IUnitOfWork unitOfWork, IMapper mapper, IHubContext<InventoryHub> inventoryHub)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _inventoryHub = inventoryHub;
        }

        private async Task<string> GenerateOrderCode()
        {
            var today = DateTime.Now.Date;
            int countToday = await _unitOfWork.OrderRepository.GetQueryable()
                .CountAsync(o => o.CreatedAt.Date == today);
            return $"HD{today:yyyyMMdd}{(countToday + 1):D4}";
        }

        [HttpPost("CreateOrderByCrash")]
        public async Task<IActionResult> CreateOrderByCrash([FromBody] OrderRequestDTO request)
        {
            if (request == null || !request.Items.Any())
                return BadRequest("Dữ liệu không hợp lệ.");

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var order = _mapper.Map<OrderEntity>(request);
                    order.Id = Guid.NewGuid();
                    order.OrderCode = await GenerateOrderCode();
                    order.CreatedAt = DateTime.Now;
                    order.Status = BackEnd.Core.Domain.Entities.OrderStatus.Completed;
                    
                    if (order.Items != null)
                    {
                        foreach (var item in order.Items)
                        {
                            item.OrderId = order.Id;
                            item.Id = Guid.NewGuid();
                        }
                    }

                    var printItems = new List<object>();

                    foreach (var itemRequest in request.Items)
                    {
                        var product = await _unitOfWork.ProductRepository.GetByIdAsync(itemRequest.ProductId);

                        if (product == null)
                        {
                            await transaction.RollbackAsync();
                            return BadRequest($"Sản phẩm với ID {itemRequest.ProductId} không tồn tại.");
                        }

                        if (product.Quantity < itemRequest.Quantity)
                        {
                            await transaction.RollbackAsync();
                            return BadRequest($"Sản phẩm '{product.Name}' không đủ số lượng trong kho (Còn lại: {product.Quantity}).");
                        }

                        product.Quantity -= itemRequest.Quantity;
                        await _unitOfWork.ProductRepository.UpdateAsync(product);

                        printItems.Add(new
                        {
                            ProductName = product.Name,
                            Quantity = itemRequest.Quantity,
                            Price = itemRequest.Price,
                            SubTotal = itemRequest.Total
                        });
                    }

                    _unitOfWork.OrderRepository.Add(order);
                    await _unitOfWork.CompleteAsync();
                    await transaction.CommitAsync();

                    var orderDto = _mapper.Map<OrderResponseDTO>(order);

                    return Ok(new
                    {
                        Message = "Tạo đơn hàng thành công",
                        Order = orderDto,
                        DataForPrint = new
                        {
                            OrderCode = order.OrderCode,
                            CreatedAt = order.CreatedAt.ToString("dd/MM/yyyy HH:mm"),
                            StaffName = User.FindFirst("FullName")?.Value ?? "N/V Bán Hàng",
                            PaymentMethod = order.PaymentMethod,
                            Items = printItems,
                            TotalAmount = order.TotalAmount,
                            FinalAmount = order.FinalAmount
                        }
                    });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
                }
            }
        }

        [HttpPost("CreateOrderOnline")]
        public async Task<IActionResult> CreateOrderOnline([FromBody] OrderRequestDTO request, [FromServices] IVNPayService vnPayService)
        {
            if (request == null || !request.Items.Any())
                return BadRequest("Dữ liệu không hợp lệ.");

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var order = _mapper.Map<OrderEntity>(request);
                    order.Id = Guid.NewGuid();
                    order.OrderCode = await GenerateOrderCode();
                    order.CreatedAt = DateTime.Now;
                    order.Status = BackEnd.Core.Domain.Entities.OrderStatus.Pending; // Trạng thái chờ thanh toán
                    
                    if (order.Items != null)
                    {
                        foreach (var item in order.Items)
                        {
                            item.OrderId = order.Id;
                            item.Id = Guid.NewGuid();
                        }
                    }

                    foreach (var itemRequest in request.Items)
                    {
                        var product = await _unitOfWork.ProductRepository.GetByIdAsync(itemRequest.ProductId);
                        if (product == null)
                        {
                            await transaction.RollbackAsync();
                            return BadRequest($"Sản phẩm với ID {itemRequest.ProductId} không tồn tại.");
                        }
                        if (product.Quantity < itemRequest.Quantity)
                        {
                            await transaction.RollbackAsync();
                            return BadRequest($"Sản phẩm '{product.Name}' không đủ số lượng trong kho (Còn lại: {product.Quantity}).");
                        }
                    }

                    _unitOfWork.OrderRepository.Add(order);
                    await _unitOfWork.CompleteAsync();
                    await transaction.CommitAsync();

                    // ✅ FIX #1: Đặt PaymentMethod = "VNPay"
                    order.PaymentMethod = "VNPay";
                    await _unitOfWork.OrderRepository.UpdateAsync(order);
                    await _unitOfWork.CompleteAsync();

                    var vnPayRequest = new BackEnd.Core.Models.Payment.VnPaymentRequestModel
                    {
                        StoreId = order.StoreId,
                        OrderId = order.Id,
                        OrderCode = order.OrderCode,
                        FullName = User.FindFirst("FullName")?.Value ?? "N/V Bán Hàng",
                        Description = $"Thanh toan don hang {order.OrderCode}",
                        Amount = (double)order.FinalAmount,
                        CreatedDate = order.CreatedAt,
                        ExpireDate = order.CreatedAt.AddMinutes(15).ToString("yyyyMMddHHmmss")
                    };

                    var paymentUrl = await vnPayService.CreatePaymentUrl(HttpContext, vnPayRequest);

                    return Ok(new
                    {
                        Message = "Tạo đơn hàng chờ thanh toán thành công",
                        OrderId = order.Id,
                        OrderCode = order.OrderCode,
                        PaymentUrl = paymentUrl
                    });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
                }
            }
        }

        [HttpGet("GetAllOrder")]
        public async Task<IActionResult> GetAllOrder()
        {
            var orders = await _unitOfWork.OrderRepository.GetAllAsync();
            var orderDtos = _mapper.Map<List<OrderResponseDTO>>(orders);
            return Ok(orderDtos);
        }

        [HttpGet("GetOrderDetail/{id}")]
        public async Task<IActionResult> GetOrderDetailById(Guid id)
        {
            var query = _unitOfWork.OrderRepository.GetQueryable();

            var orderEntity = await query
                .Include(o => o.Store)
                .Include(o => o.Employee)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (orderEntity == null)
            {
                return NotFound(new { Message = "Không tìm thấy hóa đơn" });
            }

            var result = _mapper.Map<OrderResponseDTO>(orderEntity);
            return Ok(result);
        }

        [HttpGet("GetOrderByCode/{code}")]
        public IActionResult GetOrderByCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return BadRequest("Mã hóa đơn không được để trống");

            var orders = _unitOfWork.OrderRepository.Find(o => o.OrderCode.Contains(code));
            var orderDtos = _mapper.Map<IEnumerable<OrderResponseDTO>>(orders);
            return Ok(orderDtos);
        }

        [HttpGet("GetOrdersByDate")]
        public IActionResult GetOrdersByDate([FromQuery] DateTime date)
        {
            var orders = _unitOfWork.OrderRepository.Find(o => o.CreatedAt.Date == date.Date);
            var orderDtos = _mapper.Map<IEnumerable<OrderResponseDTO>>(orders);
            return Ok(orderDtos);
        }
        [HttpPut("CancelOrder/{id}")]
        [Authorize(Roles = "RootAdmin, Manager")]
        public async Task<IActionResult> CancelOrder(Guid id)
        {
            var order = await _unitOfWork.OrderRepository.GetQueryable()
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (order == null)
            {
                return NotFound("Không tìm thấy hóa đơn.");
            }

            if (order.Status == BackEnd.Core.Domain.Entities.OrderStatus.Cancelled)
            {
                return BadRequest("Hóa đơn này đã được hủy trước đó.");
            }

            order.Status = BackEnd.Core.Domain.Entities.OrderStatus.Cancelled;

            // Hoàn số lượng sản phẩm
            foreach (var item in order.Items)
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    product.Quantity += item.Quantity;
                    await _unitOfWork.ProductRepository.UpdateAsync(product);
                }
            }

            var result = await _unitOfWork.CompleteAsync();

            if (result > 0)
            {
                await _inventoryHub.Clients.All.SendAsync("UpdateInventory", "Kho đã cập nhật");
                return Ok(new { message = "Hủy hóa đơn thành công." });
            }

            return StatusCode(500, "Có lỗi xảy ra trong quá trình cập nhật dữ liệu.");
        }
    }
}

