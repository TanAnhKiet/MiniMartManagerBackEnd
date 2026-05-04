using AutoMapper;
using BackEnd.Core.Domain.Entities;
using System;

namespace BackEnd.Core.Models.Response
{
    public class EmployeeResponseDTO
    {
        public Guid Id { get; set; }
        public string EmployeeCode { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Position { get; set; }
        public bool IsActive { get; set; }
        public Guid StoreId { get; set; }
        public Guid AppUserId { get; set; }
        public DateTime CreatedAt { get; set; }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<EmployeeEntity, EmployeeResponseDTO>().ReverseMap();
            }
        }
    }
}
