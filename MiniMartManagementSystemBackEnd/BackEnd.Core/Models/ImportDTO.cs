using AutoMapper;
using BackEnd.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BackEnd.Core.Models
{
    public class ImportDTO
    {
        public Guid Id { get; set; }
        public string? ImportCode { get; set; } // Mã phiếu nhập 
   
        public Guid StoreId { get; set; }

        public Guid SupplierId { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<ImportEntity, ImportDTO>().ReverseMap();
            }
        }
    }
}
