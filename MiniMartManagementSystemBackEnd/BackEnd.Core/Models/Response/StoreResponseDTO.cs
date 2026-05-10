using AutoMapper;
using BackEnd.Core.Domain.Entities;
using System;

namespace BackEnd.Core.Models.Response
{
    public class StoreResponseDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? LogoUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<StoreEntity, StoreResponseDTO>().ReverseMap();
            }
        }
    }
}
