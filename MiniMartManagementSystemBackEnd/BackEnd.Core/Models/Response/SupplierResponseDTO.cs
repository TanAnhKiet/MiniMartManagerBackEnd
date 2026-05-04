using AutoMapper;
using BackEnd.Core.Domain.Entities;
using System;

namespace BackEnd.Core.Models.Response
{
    public class SupplierResponseDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? ContactPerson { get; set; }
        public DateTime CreatedAt { get; set; }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<SupplierEntity, SupplierResponseDTO>().ReverseMap();
            }
        }
    }
}
