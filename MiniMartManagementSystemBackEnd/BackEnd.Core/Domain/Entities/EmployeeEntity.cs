using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using BackEnd.Core.Domain.Identity;
using System.Diagnostics.Contracts;

namespace BackEnd.Core.Domain.Entities
{
    //=======Thiết kế bảng Employee để lưu thông tin nhân viên, liên kết với AppUser qua AppUserId=======

    [Table("Employee")]
    [Index(nameof(AppUserId), IsUnique = true)] 
    public class EmployeeEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid(); // id

        // --- THÔNG TIN CÁ NHÂN (Lưu hết ở đây) ---

        public string EmployeeCode { get; set; } = null!; // Mã nhân viên tự sinh, ví dụ: EMP001

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = null!; // Họ tên đầy đủ

        [MaxLength(255)]
        public string? Address { get; set; } // Địa chỉ nhà

        [MaxLength(15)]
        public string? PhoneNumber { get; set; } // Số điện thoại liên hệ

        public DateTime? DateOfBirth { get; set; }// Ngày sinh

        // --- THÔNG TIN CÔNG VIỆC ---
        [Required]
        public string? Position { get; set; } // Ví dụ: Quản lý, Nhân viên bán hàng, Kho

        public bool IsActive { get; set; } = true; // Trạng thái làm việc

        // --- LIÊN KẾT HỆ THỐNG ---
        [Required]
        public Guid AppUserId { get; set; } // Khóa ngoại sang AppUser (Identity)

        [ForeignKey(nameof(AppUserId))]
        public virtual AppUser AppUser { get; set; } = null!;

        [Required]
        public Guid StoreId { get; set; } // Nhân viên thuộc cửa hàng nào

        [ForeignKey(nameof(StoreId))]
        public virtual StoreEntity Store { get; set; } = null!;

        public DateTime CreatedAt { get; set; }// Thời điểm tạo nhân viên

        // Các quan hệ khác
        public virtual ICollection<OrderEntity> Orders { get; set; } = new List<OrderEntity>();
    }
}
