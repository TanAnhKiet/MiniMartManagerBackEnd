using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Core.Domain.Entities
{
    
    //==== Thiết kế bảng ImportItem để lưu thông tin chi tiết về từng mặt hàng trong một lần nhập hàng, liên kết với Import qua ImportId và Product qua ProductId===

    [Table("ImportItems")]
    [Index(nameof(Id), IsUnique = true)] 
    public class ImportItemEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();//id

        [Required]
        public Guid ImportId { get; set; } //id của lần nhập hàng

        [ForeignKey("ImportId")]
        public ImportEntity Import { get; set; } = null!;

        [Required]
        public Guid ProductId { get; set; }//id của sản phẩm được nhập hàng

        [ForeignKey("ProductId")]
        public ProductEntity Product { get; set; } = null!;

        [Required]
        public int Quantity { get; set; } //số lượng sản phẩm được nhập hàng

        [Required]
        public decimal CostPrice { get; set; }//giá nhập hàng của sản phẩm

        [Required]
        public DateTime ExpiryDate { get; set; }//ngày hết hạn của sản phẩm (nếu có)

    }
}
