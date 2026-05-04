using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Core.Domain.Entities
{
    //Thiết kế bảng Category để lưu thông tin danh mục sản phẩm, liên kết với Product qua CategoryId

    [Table("Category")]
    [Index(nameof(Id), IsUnique = true)] 
    public class CategoryEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();// Id

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } =null!; // Ten danh muc

        [Required]
        public DateTime CreatedAt { get; set; } // Thời gian tạo

        public ICollection<ProductEntity> Products { get; set; } = null!; // Danh sách sản phẩm thuộc danh mục này
    }
}
