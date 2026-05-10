using BackEnd.Core.Domain.Entities;

namespace MiniMartManagementAPI.Service.Promotions
{
    public class FixedPromotionStrategy : IPromotionStrategy
    {
        public decimal CalculateDiscount(PromotionEntity promotion, decimal originalPrice, int quantity)
        {
            if (promotion.DiscountValue == null) return 0;
            
            // Nếu giá giảm lớn hơn giá gốc, thì giảm tối đa bằng giá gốc
            decimal discountPerItem = Math.Min(promotion.DiscountValue.Value, originalPrice);
            return discountPerItem * quantity;
        }
    }
}
