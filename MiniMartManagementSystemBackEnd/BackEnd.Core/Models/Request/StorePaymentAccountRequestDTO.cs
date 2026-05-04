using AutoMapper;
using BackEnd.Core.Domain.Entities;
using System;

namespace BackEnd.Core.Models.Request
{
    public class StorePaymentAccountRequestDTO
    {
        public Guid? Id { get; set; }
        public Guid StoreId { get; set; }
        public string Provider { get; set; } = null!;
        public string AccountName { get; set; } = null!;
        public string MerchantId { get; set; } = null!;
        public string PublicKey { get; set; } = null!;
        public string SecretKey { get; set; } = null!;
        public string BaseUrl { get; set; } = null!;
        public string ReturnUrl { get; set; } = null!;
        public bool IsActive { get; set; }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<StorePaymentAccountRequestDTO, StorePaymentAccountEntity>();
            }
        }
    }
}
