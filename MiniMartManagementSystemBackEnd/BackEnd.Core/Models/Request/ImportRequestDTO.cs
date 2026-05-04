using AutoMapper;
using BackEnd.Core.Domain.Entities;
using System;
using System.Collections.Generic;

namespace BackEnd.Core.Models.Request
{
    public class ImportRequestDTO
    {
        public Guid StoreId { get; set; }
        public Guid SupplierId { get; set; }
        public string? ImportCode { get; set; }
        public string Status { get; set; } = null!;
        public List<ImportItemRequest> Items { get; set; } = new List<ImportItemRequest>();

        public class ImportItemRequest
        {
            public Guid Id { get; set; } = Guid.NewGuid();
            public Guid ProductId { get; set; }
            public int Quantity { get; set; }
            public decimal CostPrice { get; set; }
            public DateTime? ExpiryDate { get; set; }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<ImportRequestDTO, ImportEntity>()
                    .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

                CreateMap<ImportItemRequest, ImportItemEntity>();
            }
        }
    }
}
