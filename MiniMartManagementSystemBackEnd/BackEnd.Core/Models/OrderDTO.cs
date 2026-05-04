using AutoMapper;
using BackEnd.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BackEnd.Core.Models
{
    public class OrderDTO
    {
      
        public Guid Id { get; set; } 

        public Guid StoreId { get; set; }

        public Guid EmployeeId { get; set; }

        public OrderStatus Status { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal FinalAmount { get; set; }

        public string PaymentMethod { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<OrderEntity, OrderDTO>().ReverseMap();
            }
        }
    }
    public enum OrderStatus
    {
        Pending,
        Completed,
        Cancelled
    }
}

