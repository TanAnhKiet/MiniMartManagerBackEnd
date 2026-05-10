using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Core.Domain.Entities
{
    // =====Thiết kế bảng Import để lưu thông tin về các lần nhập hàng, liên kết với Supplier qua SupplierId

    [Table("Imports")]
    [Index(nameof(Id), IsUnique = true)] 
    public class ImportEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();// id

        [Required]
        public string? ImportCode { get; set; } //Ma hoa don 

        [Required]
        public Guid StoreId { get; set; } // id cua cua hang

        [ForeignKey("StoreId")]
        public StoreEntity Store { get; set; } = null!;

        [Required]
        public Guid SupplierId { get; set; } // id cua nha cung cap

        [ForeignKey("SupplierId")]
        public SupplierEntity Supplier { get; set; } = null!;

        [Required]
        public Guid EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public EmployeeEntity Employee { get; set; } = null!;

        public decimal TotalAmount { get; set; } // Tong tien cua hoa don

        public DateTime CreatedAt { get; set; } // Ngay tao hoa don

        [Required]
        public string Status { get; set; } =null!; // Trạng thái của hóa đơn (ví dụ: "Pending", "Completed", "Cancelled")

        public ICollection<ImportItemEntity> Items { get; set; } = new List<ImportItemEntity>();
    }
}
