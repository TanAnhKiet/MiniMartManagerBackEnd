using BackEnd.Core.Domain.Entities;
using AutoMapper;

namespace BackEnd.Core.Models
{
    public class StoreDTO
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<StoreEntity, StoreDTO>().ReverseMap().ForMember(dest => dest.Employees, opt => opt.Ignore())
                    .ForMember(dest => dest.Products, opt => opt.Ignore())
                    .ForMember(dest => dest.Orders, opt => opt.Ignore())
                    .ForMember(dest => dest.Imports, opt => opt.Ignore())
                    .ForMember(dest => dest.PaymentAccounts, opt => opt.Ignore());
            }
        }
    }
}
