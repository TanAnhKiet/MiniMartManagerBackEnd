using AutoMapper;
using BackEnd.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BackEnd.Core.Models
{
    public class ProductDTO
    {
        public Guid Id { get; set; } 

        public Guid StoreId { get; set; }
        
        public string Name { get; set; } = null!;

        public string Barcode { get; set; } = null!;

        public Guid CategoryId { get; set; }

        public decimal SellPrice { get; set; }

        public int Quantity { get; set; } 

        public DateTime CreatedAt { get; set; }
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<ProductEntity, ProductDTO>().ReverseMap();
            }
        }
    }
}
