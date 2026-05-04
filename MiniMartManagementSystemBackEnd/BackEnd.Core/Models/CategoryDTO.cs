using AutoMapper;
using BackEnd.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BackEnd.Core.Models
{
    public class CategoryDTO
    {

   
        public string Name { get; set; } = null!;

       
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<CategoryEntity, CategoryDTO>().ReverseMap();
            }
        }
    }
}
