using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Core.Domain.Entities
{
    // ====Thiết kế bảng OrderItem để lưu thông tin về các sản phẩm trong đơn hàng, bao gồm số lượng, giá bán, và liên kết đến đơn hàng và sản phẩm tương ứng.===


    [Table("OrderItems")]
    [Index(nameof(Id), IsUnique = true)] 
    public class OrderItemEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid(); //id
        [Required]
        public Guid OrderId { get; set; }//id đơn hàng

        [ForeignKey("OrderId")]
        public OrderEntity Order { get; set; } = null!;

        [Required]
        public Guid ProductId { get; set; }//id sản phẩm

        [ForeignKey("ProductId")]
        public ProductEntity Product { get; set; } = null!;

        [Required]
        public decimal Price { get; set; }//giá bán của sản phẩm tại thời điểm đặt hàng

        [Required]
        public int Quantity { get; set; } //số lượng sản phẩm

        [Required]
        public decimal Total { get; set; }//tổng tiền cho sản phẩm này (Quantity * Price)
    }
}
