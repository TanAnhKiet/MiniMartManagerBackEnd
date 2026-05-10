using BackEnd.Core.Domain.Entities;

namespace BackEnd.Core.Models
{
    public class PromotionDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Type { get; set; } // PERCENT, FIXED, BUY_X_GET_Y
        public decimal? DiscountValue { get; set; }
        public int? BuyQuantity { get; set; }
        public int? GetQuantity { get; set; }
        public Guid? ProductId { get; set; }
        public string? ProductName { get; set; }
        public string Scope { get; set; } // BAN_HANG, NHAP_HANG
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } // ACTIVE, INACTIVE
        public Guid StoreId { get; set; }
        public DateTime CreatedAt { get; set; }

        public class MappingProfile : AutoMapper.Profile
        {
            public MappingProfile()
            {
                CreateMap<PromotionEntity, PromotionDTO>()
                    .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null));
            }
        }
    }

    public class PromotionRequestDTO
    {
        public string Name { get; set; } = null!;
        public string Type { get; set; }
        public decimal? DiscountValue { get; set; }
        public int? BuyQuantity { get; set; }
        public int? GetQuantity { get; set; }
        public Guid? ProductId { get; set; }
        public string Scope { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public Guid StoreId { get; set; }

        public class MappingProfile : AutoMapper.Profile
        {
            public MappingProfile()
            {
                CreateMap<PromotionRequestDTO, PromotionEntity>();
            }
        }
    }
}
