using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Core.Domain.Entities
{
    // ===Thiết kế bảng StorePaymentAccounts để lưu trữ thông tin tài khoản thanh toán liên kết với cửa hàng===

    [Table("StorePaymentAccounts")]
    public class StorePaymentAccountEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid(); //id

        [Required]
        public Guid StoreId { get; set; } //id cửa hàng liên kết với tài khoản thanh toán

        [ForeignKey("StoreId")]
        public StoreEntity Store { get; set; }= null!;

        [Required]
        [MaxLength(50)]
        public string Provider { get; set; }= null!;
        // VNPay, MoMo, Stripe...

        [Required]
        [MaxLength(100)]
        public string AccountName { get; set; } = null!;

        [MaxLength(100)]
        public string MerchantId { get; set; }= null!; // Ví dụ: VNPay có mã merchant

        public string PublicKey { get; set; }= null!; // ⚠️ nên encrypt

        public string SecretKey { get; set; }= null!; // ⚠️ nên encrypt

        [Required]
        [MaxLength(255)]
        public string BaseUrl { get; set; } = null!; // URL thanh toán của VNPay (ví dụ: sandbox hoặc production)

        [Required]
        [MaxLength(255)]
        public string ReturnUrl { get; set; } = null!; // URL nhận phản hồi sau khi thanh toán xong

        public bool IsActive { get; set; } = true; // Tài khoản thanh toán có đang hoạt động hay không

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
