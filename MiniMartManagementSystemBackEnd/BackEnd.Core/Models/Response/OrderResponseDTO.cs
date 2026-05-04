using AutoMapper;
using BackEnd.Core.Domain.Entities;
using System;
using System.Collections.Generic;

namespace BackEnd.Core.Models.Response
{
    public class OrderResponseDTO
    {
        public Guid Id { get; set; }
        public string OrderCode { get; set; } = null!;
        public Guid StoreId { get; set; }
        public Guid EmployeeId { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string? StoreName { get; set; }
        public string? EmployeeName { get; set; }
        public List<OrderItemResponseDTO> Items { get; set; } = new List<OrderItemResponseDTO>();

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<OrderEntity, OrderResponseDTO>()
                    .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store.Name))
                    .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FullName))
                    .ReverseMap();
            }
        }
    }

    public class OrderItemResponseDTO
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string? ProductName { get; set; }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<OrderItemEntity, OrderItemResponseDTO>()
                    .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                    .ReverseMap();
            }
        }
    }
}
