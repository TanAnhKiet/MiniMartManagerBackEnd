using AutoMapper;
using BackEnd.Core.Domain.Entities;
using System;
using System.Collections.Generic;

namespace BackEnd.Core.Models.Request
{
    public class OrderRequestDTO
    {
        public Guid? Id { get; set; }
        public string? OrderCode { get; set; }
        public Guid StoreId { get; set; }
        public Guid EmployeeId { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public List<OrderItemRequest> Items { get; set; } = new List<OrderItemRequest>();

        public class OrderItemRequest
        {
            public Guid Id { get; set; } = Guid.NewGuid();
            public Guid ProductId { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
            public decimal Total { get; set; }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<OrderRequestDTO, OrderEntity>()
                    .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

                CreateMap<OrderItemRequest, OrderItemEntity>();
            }
        }
    }
}
