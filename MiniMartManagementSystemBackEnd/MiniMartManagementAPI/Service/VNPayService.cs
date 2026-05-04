using BackEnd.Core.ConfigOption;
using BackEnd.Core.Library;
using BackEnd.Core.Models.Payment;
using BackEnd.Core.SeedWorks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace MiniMartManagementAPI.Service
{
    public class VNPayService : IVNPayService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly ILogger<VNPayService> _logger;

        public VNPayService(IConfiguration config, IUnitOfWork unitOfWork, ILogger<VNPayService> logger)
        {
            _config = config;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<string> CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model)
        {
            try
            {
                var vnpay = new VnPayLibrary();

                vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
                vnpay.AddRequestData("vnp_Command", "pay");
                
                // ✅ FIX #4: Lấy thông tin tài khoản thanh toán VNPay từ DB theo StoreId
                var storePaymentAccounts = await _unitOfWork.StorePaymentAccountRepository.GetAllByIdAsync(model.StoreId);
                var storePaymentAccount = storePaymentAccounts?.FirstOrDefault(a => a.Provider == "VNPay" && a.IsActive);

                // ✅ FIX #5: Bắt buộc phải có setting trong DB, không được fallback
                if (storePaymentAccount == null)
                {
                    _logger.LogError($"❌ CRITICAL: VNPay payment account not configured for store: {model.StoreId}. Please configure in Settings.");
                    throw new Exception($"VNPay chưa được cấu hình cho cửa hàng này. Vui lòng liên hệ quản trị viên.");
                }

                string tmnCode = storePaymentAccount.MerchantId;
                string hashSecret = storePaymentAccount.SecretKey;
                string baseUrl = storePaymentAccount.BaseUrl;
                string returnUrl = storePaymentAccount.ReturnUrl;

                if (string.IsNullOrEmpty(tmnCode) || string.IsNullOrEmpty(hashSecret))
                {
                    _logger.LogError($"❌ CRITICAL: VNPay TmnCode or SecretKey is empty for store: {model.StoreId}");
                    throw new Exception("VNPay configuration incomplete. Please check Merchant ID and Secret Key.");
                }

                vnpay.AddRequestData("vnp_TmnCode", tmnCode);
                vnpay.AddRequestData("vnp_Amount", (model.Amount * 100).ToString()); // Số tiền nhân với 100
                vnpay.AddRequestData("vnp_CreateDate", model.CreatedDate.ToString("yyyyMMddHHmmss"));
                vnpay.AddRequestData("vnp_CurrCode", "VND");
                vnpay.AddRequestData("vnp_IpAddr", VnPayLibrary.GetIpAddress(context));
                vnpay.AddRequestData("vnp_Locale", "vn");

                vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + model.OrderId);
                vnpay.AddRequestData("vnp_OrderType", "other");
                vnpay.AddRequestData("vnp_ReturnUrl", returnUrl);
                vnpay.AddRequestData("vnp_TxnRef", model.OrderId.ToString()); 

                if (!string.IsNullOrEmpty(model.ExpireDate))
                {
                    vnpay.AddRequestData("vnp_ExpireDate", model.ExpireDate);
                }

                var paymentUrl = vnpay.CreateRequestUrl(baseUrl, hashSecret);

                _logger.LogInformation($"✅ Payment URL created for Order: {model.OrderCode}, Amount: {model.Amount}, Store: {model.StoreId}");
                return paymentUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error creating payment URL for Order: {model.OrderCode}");
                throw;
            }
        }

        public async Task<VnPaymentResponseModel> PaymentExecute(IQueryCollection collections)
        {
            try
            {
                var vnpay = new VnPayLibrary();
                foreach (var (key, value) in collections)
                {
                    if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(key, value.ToString());
                    }
                }

                var vnp_TmnCode = vnpay.GetResponseData("vnp_TmnCode");
                var vnp_orderId = Convert.ToString(vnpay.GetResponseData("vnp_TxnRef"));
                var vnp_TransactionId = Convert.ToString(vnpay.GetResponseData("vnp_TransactionNo"));
                var vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
                var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");
                var vnp_Amount = Convert.ToDouble(vnpay.GetResponseData("vnp_Amount")) / 100;
                var vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");

                _logger.LogInformation($"Processing VNPay response - OrderId: {vnp_orderId}, ResponseCode: {vnp_ResponseCode}, TransactionId: {vnp_TransactionId}, Amount: {vnp_Amount}");

                // Tìm HashSecret từ DB dựa trên vnp_TmnCode
                var storePaymentAccount = await _unitOfWork.StorePaymentAccountRepository.GetByMerchantIdAsync(vnp_TmnCode);

                string hashSecret = storePaymentAccount?.SecretKey ?? _config["Vnpay:HashSecret"] ?? "";

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash!, hashSecret);
                if (!checkSignature)
                {
                    _logger.LogWarning($"VNPay signature validation failed for Order: {vnp_orderId}");
                    return new VnPaymentResponseModel
                    {
                        Success = false
                    };
                }

                _logger.LogInformation($"VNPay signature validated successfully for Order: {vnp_orderId}");

                return new VnPaymentResponseModel
                {
                    Success = true,
                    PaymentMethod = "VnPay",
                    OrderDescription = vnp_OrderInfo,
                    OrderId = vnp_orderId,
                    TransactionId = vnp_TransactionId,
                    Token = vnp_SecureHash!,
                    VnPayResponseCode = vnp_ResponseCode,
                    vnp_Amount = vnp_Amount,
                    vnp_TransactionStatus = vnp_TransactionStatus
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing payment response from VNPay");
                throw;
            }
        }
    }
}
