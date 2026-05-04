using AutoMapper;
using BackEnd.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BackEnd.Core.Models
{
    public class StorePaymentAccountDTO
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid StoreId { get; set; }

        public string Provider { get; set; } = null!;

        public string AccountName { get; set; } = null!;

        public string MerchantId { get; set; } = null!; // Ví dụ: VNPay có mã merchant

        public string PublicKey { get; set; } = null!; // ⚠️ nên encrypt

        public string SecretKey { get; set; } = null!; // ⚠️ nên encrypt

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<StorePaymentAccountEntity, StorePaymentAccountDTO>().ReverseMap();
            }
        }
    }
}

