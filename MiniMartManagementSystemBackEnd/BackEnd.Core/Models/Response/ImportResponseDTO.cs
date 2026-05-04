using AutoMapper;
using BackEnd.Core.Domain.Entities;
using System;
using System.Collections.Generic;

namespace BackEnd.Core.Models.Response
{
    public class ImportResponseDTO
    {
        public Guid Id { get; set; }
        public string? ImportCode { get; set; }
        public Guid StoreId { get; set; }
        public Guid SupplierId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? StoreName { get; set; }
        public string? SupplierName { get; set; }
        public string? Status { get; set; }
        public List<ImportItemResponseDTO> Items { get; set; } = new List<ImportItemResponseDTO>();

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<ImportEntity, ImportResponseDTO>()
                    .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store.Name))
                    .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier.Name))
                    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                    .ReverseMap();
            }
        }
    }

    public class ImportItemResponseDTO
    {
        public Guid Id { get; set; }
        public Guid ImportId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal CostPrice { get; set; }
        public string? ProductName { get; set; }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<ImportItemEntity, ImportItemResponseDTO>()
                    .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                    .ReverseMap();
            }
        }
    }
}
