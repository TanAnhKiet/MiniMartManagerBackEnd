using AutoMapper;
using BackEnd.Core.Domain.Entities;
using System;

namespace BackEnd.Core.Models.Response
{
    public class ProductResponseDTO
    {
        public Guid Id { get; set; }
        public Guid StoreId { get; set; }
        public string Name { get; set; } = null!;
        public string Barcode { get; set; } = null!;
        public Guid CategoryId { get; set; }
        public decimal SellPrice { get; set; }
        public int Quantity { get; set; }
        public int WarningThreshold { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime CreatedAt { get; set; }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<ProductEntity, ProductResponseDTO>().ReverseMap();
            }
        }
    }
}
