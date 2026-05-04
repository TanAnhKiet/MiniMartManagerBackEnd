using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Core.Domain.Entities
{
    //==== Thiết kế bảng Order để lưu thông tin đơn hàng, liên kết với Store và Employee===

    [Table("Orders")]
    [Index(nameof(Id), IsUnique = true)] 
    public class OrderEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();// id
        [Required]
        public string OrderCode { get; set; } = null!;

        [Required]
        public Guid StoreId { get; set; }// id cửa hàng

        [ForeignKey("StoreId")]
        public StoreEntity Store { get; set; } = null!;

        [Required]
        public Guid EmployeeId { get; set; }// id nhân viên

        [ForeignKey("EmployeeId")]
        public EmployeeEntity Employee { get; set; } = null!;

        public OrderStatus Status { get; set; }// trạng thái đơn hàng

        public decimal TotalAmount { get; set; }// tổng tiền trước khi áp dụng giảm giá

        public decimal FinalAmount { get; set; }// tổng tiền sau khi áp dụng giảm giá

        public string PaymentMethod { get; set; }= null!;// phương thức thanh toán

        public DateTime CreatedAt { get; set; } // ngày tạo đơn hàng

        public ICollection<OrderItemEntity> Items { get; set; } = new List<OrderItemEntity>();
    }
    public enum OrderStatus // trạng thái đơn hàng
    {
        Pending,
        Completed,
        Cancelled
    }
}
