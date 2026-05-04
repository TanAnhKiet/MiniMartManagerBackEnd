using AutoMapper;
using BackEnd.Core.Domain.Entities;
using System;

namespace BackEnd.Core.Models.Response
{
    public class StorePaymentAccountResponseDTO
    {
        public Guid Id { get; set; }
        public Guid StoreId { get; set; }
        public string Provider { get; set; } = null!;
        public string AccountName { get; set; } = null!;
        public string MerchantId { get; set; } = null!;
        public string PublicKey { get; set; } = null!;
        public string SecretKey { get; set; } = null!;
        public string BaseUrl { get; set; } = null!;
        public string ReturnUrl { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // VNPay Aliases for UI display
        public string? TmnCode => Provider == "VNPay" ? MerchantId : null;
        public string? HashSecret => Provider == "VNPay" ? SecretKey : null;

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<StorePaymentAccountEntity, StorePaymentAccountResponseDTO>().ReverseMap();
            }
        }
    }
}
