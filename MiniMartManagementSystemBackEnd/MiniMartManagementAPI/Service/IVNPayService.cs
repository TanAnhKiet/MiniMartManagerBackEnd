using BackEnd.Core.Models.Payment;
using Microsoft.AspNetCore.Http;

namespace MiniMartManagementAPI.Service
{
    public interface IVNPayService
    {
        Task<string> CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model);
        Task<VnPaymentResponseModel> PaymentExecute(IQueryCollection collections);
    }
}
