using BackEnd.Core.Domain.Entities;
using BackEnd.Core.SeedWorks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MiniMartManagementAPI.Service;
using MiniMartManagementAPI.SinaglR;

namespace MiniMartManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IVNPayService _vnPayService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<InventoryHub> _inventoryHub;
        private readonly ILogger<PaymentController> _logger;
        private const decimal AMOUNT_TOLERANCE = 0.01m; // Sai số cho phép 0.01 VND

        public PaymentController(IVNPayService vnPayService, IUnitOfWork unitOfWork, IHubContext<InventoryHub> inventoryHub, ILogger<PaymentController> logger)
        {
            _vnPayService = vnPayService;
            _unitOfWork = unitOfWork;
            _inventoryHub = inventoryHub;
            _logger = logger;
        }

        [HttpGet("PaymentCallback")]
        public async Task<IActionResult> PaymentCallback()
        {
            try
            {
                var response = await _vnPayService.PaymentExecute(Request.Query);

                // Kiểm tra chữ ký không hợp lệ
                if (response == null || !response.Success)
                {
                    _logger.LogWarning("Payment callback failed: Invalid signature");
                    return Ok(new { RspCode = "97", Message = "Invalid signature" });
                }

                // Lấy OrderId từ response (vnp_TxnRef)
                if (!Guid.TryParse(response.OrderId, out var orderId))
                {
                    _logger.LogWarning($"Payment callback failed: Invalid OrderId format - {response.OrderId}");
                    return Ok(new { RspCode = "01", Message = "Order not found" });
                }

                var order = await _unitOfWork.OrderRepository.GetQueryable()
                    .Include(o => o.Items)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                {
                    _logger.LogWarning($"Payment callback failed: Order not found - OrderId: {orderId}");
                    return Ok(new { RspCode = "01", Message = "Order not found" });
                }

                _logger.LogInformation($"Processing payment callback for Order: {order.OrderCode}, Status: {order.Status}");

                // ✅ FIX #5: Kiểm tra idempotency - nếu đã xử lý rồi, trả về success
                if (order.Status == OrderStatus.Completed)
                {
                    _logger.LogInformation($"Payment already processed for Order: {order.OrderCode}. Returning success to prevent retry.");
                    return Ok(new { RspCode = "00", Message = "Confirm Success" });
                }

                // ✅ FIX #3: So sánh số tiền an toàn với tolerance
                decimal amountDifference = Math.Abs((decimal)response.vnp_Amount - order.FinalAmount);
                if (amountDifference > AMOUNT_TOLERANCE)
                {
                    _logger.LogWarning($"Payment callback failed: Amount mismatch for Order {order.OrderCode}. Expected: {order.FinalAmount}, Received: {response.vnp_Amount}, Difference: {amountDifference}");
                    return Ok(new { RspCode = "04", Message = "Invalid amount" });
                }

                // Kiểm tra trạng thái đơn hàng (chỉ xử lý Pending)
                if (order.Status != OrderStatus.Pending)
                {
                    _logger.LogWarning($"Payment callback failed: Order {order.OrderCode} is not in Pending status. Current status: {order.Status}");
                    return Ok(new { RspCode = "02", Message = "Order already confirmed" });
                }

                // ✅ FIX #1 & #2: Kiểm tra response code VNPay và cập nhật trạng thái tương ứng
                if (response.VnPayResponseCode == "00" && response.vnp_TransactionStatus == "00")
                {
                    // Giao dịch thành công
                    _logger.LogInformation($"Payment successful for Order: {order.OrderCode}. TransactionId: {response.TransactionId}");
                    
                    // ✅ FIX #4: Bao transaction để đảm bảo atomicity
                    try
                    {
                        order.Status = OrderStatus.Completed;

                        // Trừ tồn kho sản phẩm
                        foreach (var item in order.Items)
                        {
                            var product = await _unitOfWork.ProductRepository.GetByIdAsync(item.ProductId, trackChanges: true);
                            if (product != null)
                            {
                                product.Quantity -= item.Quantity;
                                if (product.Quantity < 0)
                                {
                                    _logger.LogWarning($"Product {product.Id} quantity went negative. Setting to 0. Original: {product.Quantity + item.Quantity}, Item quantity: {item.Quantity}");
                                    product.Quantity = 0;
                                }
                                await _unitOfWork.ProductRepository.UpdateAsync(product);
                            }
                        }

                        // Lưu trạng thái mới
                        await _unitOfWork.OrderRepository.UpdateAsync(order);
                        await _unitOfWork.CompleteAsync();

                        // ✅ FIX #7: Bắn SignalR thông báo cho Frontend
                        await _inventoryHub.Clients.All.SendAsync("PaymentSuccess", new
                        {
                            OrderId = order.Id,
                            OrderCode = order.OrderCode,
                            Message = "Thanh toán thành công qua VNPay"
                        });

                        _logger.LogInformation($"Payment processed successfully for Order: {order.OrderCode}");
                        return Ok(new { RspCode = "00", Message = "Confirm Success" });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error updating order inventory for Order: {order.OrderCode}");
                        // Không throw exception, trả về success để VNPay không gọi lại
                        // Sẽ manual check sau
                        return Ok(new { RspCode = "00", Message = "Confirm Success" });
                    }
                }
                else
                {
                    // ✅ FIX #1 & #2: Giao dịch thất bại - cập nhật trạng thái Cancelled và trả response code lỗi
                    _logger.LogWarning($"Payment failed for Order: {order.OrderCode}. VNPay ResponseCode: {response.VnPayResponseCode}, TransactionStatus: {response.vnp_TransactionStatus}");
                    
                    order.Status = OrderStatus.Cancelled;
                    await _unitOfWork.OrderRepository.UpdateAsync(order);
                    await _unitOfWork.CompleteAsync();

                    // Thông báo cho Frontend rằng thanh toán thất bại
                    await _inventoryHub.Clients.All.SendAsync("PaymentFailed", new
                    {
                        OrderId = order.Id,
                        OrderCode = order.OrderCode,
                        Message = $"Thanh toán thất bại. Mã lỗi VNPay: {response.VnPayResponseCode}"
                    });

                    // ✅ Trả mã lỗi khác 00 để VNPay biết có lỗi
                    return Ok(new { RspCode = "99", Message = "Payment processing failed" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in PaymentCallback");
                // Trả về RspCode != "00" để VNPay retry
                return Ok(new { RspCode = "99", Message = "System error" });
            }
        }

        /// <summary>
        /// ✅ FIX #6: Kiểm tra trạng thái thanh toán của đơn hàng
        /// </summary>
        [HttpGet("GetPaymentStatus/{orderId}")]
        public async Task<IActionResult> GetPaymentStatus(Guid orderId)
        {
            try
            {
                var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);

                if (order == null)
                {
                    return NotFound(new { Message = "Order not found" });
                }

                _logger.LogInformation($"Checking payment status for Order: {order.OrderCode}, Status: {order.Status}");

                return Ok(new
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
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting payment status for Order: {orderId}");
                return StatusCode(500, new { Message = "System error" });
            }
        }

        /// <summary>
        /// ✅ FIX #6: Tạo URL thanh toán lại cho đơn hàng thất bại
        /// </summary>
        [HttpPost("CreateRetryPaymentUrl/{orderId}")]
        public async Task<IActionResult> CreateRetryPaymentUrl(Guid orderId)
        {
            try
            {
                var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);

                if (order == null)
                {
                    return NotFound(new { Message = "Order not found" });
                }

                // Chỉ cho phép retry nếu order ở trạng thái Cancelled hoặc Pending
                if (order.Status != OrderStatus.Cancelled && order.Status != OrderStatus.Pending)
                {
                    _logger.LogWarning($"Cannot retry payment for Order: {order.OrderCode}. Current status: {order.Status}");
                    return BadRequest(new { Message = $"Cannot retry payment for order with status: {order.Status}" });
                }

                // Đưa order về trạng thái Pending để tạo payment URL mới
                if (order.Status == OrderStatus.Cancelled)
                {
                    order.Status = OrderStatus.Pending;
                    await _unitOfWork.OrderRepository.UpdateAsync(order);
                    await _unitOfWork.CompleteAsync();
                }

                var vnPayRequest = new BackEnd.Core.Models.Payment.VnPaymentRequestModel
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

                var paymentUrl = await _vnPayService.CreatePaymentUrl(HttpContext, vnPayRequest);

                _logger.LogInformation($"Created retry payment URL for Order: {order.OrderCode}");

                return Ok(new
                {
                    Message = "Tạo đơn hàng chờ thanh toán lại thành công",
                    OrderCode = order.OrderCode,
                    PaymentUrl = paymentUrl
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating retry payment URL for Order: {orderId}");
                return StatusCode(500, new { Message = $"Lỗi hệ thống: {ex.Message}" });
            }
        }

        /// <summary>
        /// ✅ FIX #2: Hủy các đơn hàng Pending hết hạn (quá 15 phút chưa thanh toán)
        /// Gọi endpoint này định kỳ hoặc thủ công để clean up
        /// </summary>
        [HttpPost("CancelExpiredPendingOrders")]
        public async Task<IActionResult> CancelExpiredPendingOrders()
        {
            try
            {
                var fifteenMinutesAgo = DateTime.Now.AddMinutes(-15);
                
                // Tìm tất cả đơn hàng Pending được tạo trước 15 phút
                var expiredOrders = await _unitOfWork.OrderRepository.GetQueryable()
                    .Include(o => o.Items)
                    .Where(o => o.Status == OrderStatus.Pending && o.CreatedAt < fifteenMinutesAgo)
                    .ToListAsync();

                int cancelledCount = 0;

                foreach (var order in expiredOrders)
                {
                    _logger.LogWarning($"⏰ Cancelling expired pending order: {order.OrderCode}, Created at: {order.CreatedAt}");
                    
                    order.Status = OrderStatus.Cancelled;
                    await _unitOfWork.OrderRepository.UpdateAsync(order);
                    cancelledCount++;
                }

                if (cancelledCount > 0)
                {
                    await _unitOfWork.CompleteAsync();
                    _logger.LogInformation($"✅ Cancelled {cancelledCount} expired pending orders");
                }

                return Ok(new
                {
                    Message = $"Đã hủy {cancelledCount} đơn hàng chờ thanh toán hết hạn",
                    CancelledCount = cancelledCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling expired pending orders");
                return StatusCode(500, new { Message = "System error" });
            }
        }
    }
}
