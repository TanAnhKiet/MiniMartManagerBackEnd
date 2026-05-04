using AutoMapper;
using BackEnd.Core.Domain.Entities;
using System;

namespace BackEnd.Core.Models.Request
{
    public class CategoryRequestDTO
    {
        public Guid? Id { get; set; }
        public string Name { get; set; } = null!;

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<CategoryRequestDTO, CategoryEntity>();
            }
        }
    }
}
