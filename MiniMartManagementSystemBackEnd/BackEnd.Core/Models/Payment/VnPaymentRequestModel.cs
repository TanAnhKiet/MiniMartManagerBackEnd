using System;

namespace BackEnd.Core.Models.Payment
{
    public class VnPaymentRequestModel
    {
        public Guid StoreId { get; set; }
        public Guid OrderId { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Amount { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? ExpireDate { get; set; } // yyyyMMddHHmmss
    }
}
