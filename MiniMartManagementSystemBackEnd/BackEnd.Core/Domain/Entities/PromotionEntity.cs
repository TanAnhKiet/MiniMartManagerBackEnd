using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Core.Domain.Entities
{
    public enum PromotionType
    {
        PERCENT,    // %
        FIXED,      // Số tiền cố định
        BUY_X_GET_Y // Mua X tặng Y
    }

    public enum PromotionScope
    {
        BAN_HANG,   // POS
        NHAP_HANG   // Import
    }

    public enum PromotionStatus
    {
        ACTIVE,
        INACTIVE
    }

    [Table("Promotions")]
    [Index(nameof(Id), IsUnique = true)]
    public class PromotionEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = null!; // Tên chương trình KM

        [Required]
        public PromotionType Type { get; set; }

        public decimal? DiscountValue { get; set; } // % hoặc số tiền giảm

        public int? BuyQuantity { get; set; } // Chỉ dùng khi BUY_X_GET_Y

        public int? GetQuantity { get; set; } // Chỉ dùng khi BUY_X_GET_Y

        public Guid? ProductId { get; set; } // FK → Product (Nếu KM cho sp cụ thể)

        [ForeignKey(nameof(ProductId))]
        public virtual ProductEntity? Product { get; set; }

        [Required]
        public PromotionScope Scope { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public PromotionStatus Status { get; set; }

        [Required]
        public Guid StoreId { get; set; }

        [ForeignKey(nameof(StoreId))]
        public virtual StoreEntity Store { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid? CreatedById { get; set; }

        [ForeignKey(nameof(CreatedById))]
        public virtual EmployeeEntity? CreatedBy { get; set; }
    }
}
