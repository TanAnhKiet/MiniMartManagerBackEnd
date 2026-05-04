using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Core.Domain.Identity
{
    // Chỉnh sửa lớp AppUser để thêm các thuộc tính cần thiết cho việc quản lý người dùng, bao gồm trạng thái hoạt động, ngày tạo tài khoản, token làm mới và thời gian hết hạn của token làm mới.
    // Lớp này chỉ phục vụ cho mục đích quản lý người dùng và xác thực, không chứa thông tin liên quan đến vai trò hay quyền hạn của người dùng, những thông tin này sẽ được quản lý thông qua các bảng liên kết giữa AppUser và AppRole.
    [Table("AppUser")]
    public class AppUser : IdentityUser<Guid>
    {
        public bool IsActive { get; set; } = true; // Trạng thái hoạt động của người dùng
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Ngày tạo tài khoản
        public string? RefreshToken { get; set; } // Token làm mới để xác thực lại
        public DateTime RefreshTokenExpiryTime { get; set; } // Thời gian hết hạn của token làm mới
        public string FullName { get; set; } = null!; // Họ và tên đầy đủ của người dùng
       
    }
}
