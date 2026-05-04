using AutoMapper;
using BackEnd.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BackEnd.Core.Models
{
    public class ImportItemDTO
    {
        public Guid Id { get; set; }

        public Guid ImportId { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal CostPrice { get; set; }

        public DateTime ExpiryDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<ImportItemEntity, ImportItemDTO>().ReverseMap();
            }
        }
    }
}
