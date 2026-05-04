using AutoMapper;
using BackEnd.Core.Domain.Entities;
using System;

namespace BackEnd.Core.Models.Request
{
    public class ProductRequestDTO
    {
        public Guid? Id { get; set; }
        public Guid StoreId { get; set; }
        public string Name { get; set; } = null!;
        public string Barcode { get; set; } = null!;
        public Guid CategoryId { get; set; }
        public decimal SellPrice { get; set; }
        public int Quantity { get; set; }
        public DateTime? ExpiryDate { get; set; }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<ProductRequestDTO, ProductEntity>();
            }
        }
    }
}
