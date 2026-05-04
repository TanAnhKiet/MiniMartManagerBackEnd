using AutoMapper;
using BackEnd.Core.Domain.Entities;
using System;

namespace BackEnd.Core.Models.Request
{
    public class EmployeeRequestDTO
    {
        public string FullName { get; set; } = null!;
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Position { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid StoreId { get; set; }
        public string Password { get; set; } = null!;

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<EmployeeRequestDTO, EmployeeEntity>()
                    .ForMember(dest => dest.Orders, opt => opt.Ignore());
            }
        }
    }
}
