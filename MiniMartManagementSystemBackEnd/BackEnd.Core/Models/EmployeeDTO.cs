using AutoMapper;
using BackEnd.Core.Domain.Entities;
using BackEnd.Core.Domain.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace BackEnd.Core.Models
{
    public class EmployeeDTO
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string FullName { get; set; } = null!;
  
        public string? Address { get; set; }

        public string? PhoneNumber { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? Position { get; set; } // Ví dụ: Quản lý, Nhân viên bán hàng, Kho

        public bool IsActive { get; set; } = true; // Trạng thái làm việc

        public Guid AppUserId { get; set; } // Khóa ngoại sang AppUser (Identity)

        public Guid StoreId { get; set; } // Nhân viên thuộc cửa hàng nào

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<EmployeeEntity, EmployeeDTO>().ReverseMap();
            }
        }
    }
}
