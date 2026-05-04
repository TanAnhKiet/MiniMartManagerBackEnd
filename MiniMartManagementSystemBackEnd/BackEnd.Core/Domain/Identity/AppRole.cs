using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Core.Domain.Identity
{
    /// Chỉnh sửa lớp AppRole để thêm thuộc tính DisplayName, giúp phân biệt giữa tên kỹ thuật (Name) và tên hiển thị của vai trò.
    [Table("AppRole")]
    public class AppRole :IdentityRole<Guid>
    {
        [Required]
        [MaxLength(255)]
        public required string DisplayName { get; set; }// Tên hiển thị của vai trò, có thể khác với tên kỹ thuật (Name)
    }
}
