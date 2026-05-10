using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Core.Domain.Entities
{
    //==== Thiết kế bảng Store để lưu thông tin về cửa hàng, bao gồm tên, địa chỉ, số điện thoại, email và thời gian tạo.===
    [Table("Stores")]
    [Index(nameof(Id), IsUnique = true)]
    public class StoreEntity
    {

        [Key]
        public Guid Id { get; set; } = Guid.NewGuid(); //id

        [Required, MaxLength(200)]
        public string Name { get; set; }=null!; // Tên cửa hàng

        [Required, MaxLength(200)]
        public string Address { get; set; } = null!;// Địa chỉ cửa hàng

        [Required, MaxLength(20)]
        public string Phone { get; set; } = null!;// Số điện thoại cửa hàng

        [Required, MaxLength(100)]  
        public string Email { get; set; } = null!;// Email cửa hàng

        public string? LogoUrl { get; set; } // URL hoặc đường dẫn logo cửa hàng

        public DateTime CreatedAt { get; set; }// Thời gian tạo cửa hàng

        public ICollection<EmployeeEntity> Employees { get; set; } = new List<EmployeeEntity>();// Một cửa hàng có thể có nhiều nhân viên
        public ICollection<ProductEntity> Products { get; set; } = new List<ProductEntity>();// Một cửa hàng có thể có nhiều sản phẩm
        public ICollection<OrderEntity> Orders { get; set; } = new List<OrderEntity>();// Một cửa hàng có thể có nhiều đơn hàng
        public ICollection<ImportEntity> Imports { get; set; } = new List<ImportEntity>();// Một cửa hàng có thể có nhiều phiếu nhập hàng
        public ICollection<StorePaymentAccountEntity> PaymentAccounts { get; set; } = new List<StorePaymentAccountEntity>();// Một cửa hàng có thể có nhiều tài khoản thanh toán
    }
}
