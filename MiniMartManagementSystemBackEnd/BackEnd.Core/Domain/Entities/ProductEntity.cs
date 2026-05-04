using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Core.Domain.Entities
{
    // ===Thiết kế bảng Product để lưu thông tin về sản phẩm, bao gồm tên, mã vạch, giá bán, và liên kết đến danh mục sản phẩm.===

    [Table("Products")]
    [Index(nameof(Id), IsUnique = true)] 
    public class ProductEntity
    {
        [Key]
        public Guid Id { get; set; } =Guid.NewGuid();

        [Required]
        public Guid StoreId { get; set; } // ma cua hang

        [ForeignKey("StoreId")]
        public StoreEntity Store { get; set; } = null!;

        [Required]
        public string Name { get; set; } = null!; // ten san pham

        public string Barcode { get; set; } = null!; // ma vach, có thể null nếu sản phẩm không có mã vạch

        public Guid CategoryId { get; set; } // ma danh muc san pham

        [ForeignKey("CategoryId")]
        public CategoryEntity Category { get; set; } = null!;

        [Required]
        public decimal SellPrice { get; set; } // gia ban, có thể null nếu sản phẩm chưa được định giá

        public int Quantity { get; set; } // 🔥 tồn kho

        public DateTime? ExpiryDate { get; set; } // optional

        public DateTime CreatedAt { get; set; }

       
        public virtual ICollection<OrderItemEntity> OrderItems { get; set; } = new List<OrderItemEntity>();
    }
}
