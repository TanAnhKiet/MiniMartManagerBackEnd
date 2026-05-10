using BackEnd.Core.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniMartManagementAPI.Service.POS;
using System.Security.Claims;

namespace MiniMartManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class POSController : ControllerBase
    {
        private readonly IPOSService _posService;

        public POSController(IPOSService posService)
        {
            _posService = posService;
        }

        [HttpPost("CreateOrderByCash")]
        public async Task<IActionResult> CreateOrderByCash([FromBody] OrderRequestDTO request)
        {
            var staffName = User.FindFirstValue(ClaimTypes.Name) ?? "Nhân viên";
            var result = await _posService.CreateOrderByCash(request, staffName);
            return Ok(result);
        }

        [HttpPost("CreateOrderOnline")]
        public async Task<IActionResult> CreateOrderOnline([FromBody] OrderRequestDTO request)
        {
            var staffName = User.FindFirstValue(ClaimTypes.Name) ?? "Nhân viên";
            var result = await _posService.CreateOrderOnline(request, staffName, HttpContext);
            return Ok(result);
        }

        [HttpGet("GetAllOrders")]
        public async Task<IActionResult> GetAllOrders()
        {
            var result = await _posService.GetAllOrders();
            return Ok(result);
        }

        [HttpGet("GetOrderById/{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var result = await _posService.GetOrderById(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("PaymentCallback")]
        [AllowAnonymous]
        public async Task<IActionResult> PaymentCallback()
        {
            var result = await _posService.ProcessPaymentCallback(Request.Query);
            return Ok(result);
        }

        [HttpGet("GetPaymentStatus/{orderId}")]
        public async Task<IActionResult> GetPaymentStatus(Guid orderId)
        {
            var result = await _posService.GetPaymentStatus(orderId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("CreateRetryPaymentUrl/{orderId}")]
        public async Task<IActionResult> CreateRetryPaymentUrl(Guid orderId)
        {
            var result = await _posService.CreateRetryPaymentUrl(orderId, HttpContext);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("CancelOrder/{id}")]
        public async Task<IActionResult> CancelOrder(Guid id)
        {
            var result = await _posService.CancelOrder(id);
            if (!result) return BadRequest("Không thể hủy đơn hàng");
            return Ok(new { Message = "Đã hủy đơn hàng thành công" });
        }
    }
}
