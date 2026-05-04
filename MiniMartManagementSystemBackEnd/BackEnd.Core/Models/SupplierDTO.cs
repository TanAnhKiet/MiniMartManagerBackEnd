using AutoMapper;
using BackEnd.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BackEnd.Core.Models
{
    public class SupplierDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public string Address { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<SupplierEntity, SupplierDTO>().ReverseMap();
            }
        }
    }
}
