using BackEnd.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BackEnd.Core.Models
{
    // Model nay dung de tra ve 
    public class ImportDetailDTO
    {
        public Guid Id { get; set; } // id

        public string? ImportCode { get; set; } //Ma hoa don 

        public Guid StoreId { get; set; } // id cua cua hang

        [ForeignKey("StoreId")]
        public StoreEntity Store { get; set; } = null!;

        [Required]
        public Guid SupplierId { get; set; } // id cua nha cung cap

        [ForeignKey("SupplierId")]
        public SupplierEntity Supplier { get; set; } = null!;

        public decimal TotalAmount { get; set; } // Tong tien cua hoa don

        public DateTime CreatedAt { get; set; } // Ngay tao hoa don

        public ICollection<ImportItemEntity> Items { get; set; } = new List<ImportItemEntity>();
    }
}
