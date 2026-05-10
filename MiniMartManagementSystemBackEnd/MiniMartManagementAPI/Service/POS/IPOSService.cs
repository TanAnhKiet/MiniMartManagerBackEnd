using BackEnd.Core.Models.Request;
using BackEnd.Core.Models.Response;
using Microsoft.AspNetCore.Http;

namespace MiniMartManagementAPI.Service.POS
{
    public interface IPOSService
    {
        // Đơn hàng
        Task<OrderResponseDTO> CreateOrderByCash(OrderRequestDTO request, string staffName);
        Task<object> CreateOrderOnline(OrderRequestDTO request, string staffName, HttpContext httpContext);
        Task<IEnumerable<OrderResponseDTO>> GetAllOrders();
        Task<OrderResponseDTO> GetOrderById(Guid id);
        Task<IEnumerable<OrderResponseDTO>> GetOrdersByCode(string code);
        Task<IEnumerable<OrderResponseDTO>> GetOrdersByDate(DateTime date);
        Task<bool> CancelOrder(Guid id);

        // Thanh toán VNPay
        Task<object> ProcessPaymentCallback(IQueryCollection query);
        Task<object> GetPaymentStatus(Guid orderId);
        Task<object> CreateRetryPaymentUrl(Guid orderId, HttpContext httpContext);
        Task<int> CancelExpiredPendingOrders();
    }
}
