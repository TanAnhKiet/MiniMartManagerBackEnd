using AutoMapper;
using BackEnd.Core.Domain.Entities;

namespace BackEnd.Core.Models
{
    public class OrderItemDTO
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid OrderId { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal Total { get; set; }
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<OrderItemEntity, OrderItemDTO>().ReverseMap();
            }
        }
    }
}
