using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Core.Domain.Entities
{
    //=== Thiết kế bảng Supplier để lưu thông tin về nhà cung cấp, bao gồm tên, số điện thoại, địa chỉ và thời gian tạo.===

    [Table("Suppliers")]
    [Index(nameof(Id), IsUnique = true)] 
    public class SupplierEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid(); //id

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!;// Tên nhà cung cấp

        [MaxLength(20)]
        public string Phone { get; set; }= null!;// Số điện thoại nhà cung cấp

        [MaxLength(300)]
        public string Address { get; set; }=null!;// Địa chỉ nhà cung cấp

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;// Thời gian tạo nhà cung cấp

        public ICollection<ImportEntity> Imports { get; set; }= new List<ImportEntity>();// Một nhà cung cấp có thể có nhiều phiếu nhập hàng
    }
}
