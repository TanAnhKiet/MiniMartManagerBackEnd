using AutoMapper;
using BackEnd.Core.Domain.Entities;
using System;

namespace BackEnd.Core.Models.Request
{
    public class StoreRequestDTO
    {
        public Guid? Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<StoreRequestDTO, StoreEntity>();
            }
        }
    }
}
