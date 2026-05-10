using BackEnd.Core.Domain.Entities;

namespace MiniMartManagementAPI.Service.Promotions
{
    public class PercentPromotionStrategy : IPromotionStrategy
    {
        public decimal CalculateDiscount(PromotionEntity promotion, decimal originalPrice, int quantity)
        {
            if (promotion.DiscountValue == null) return 0;
            
            decimal discountPerItem = originalPrice * (promotion.DiscountValue.Value / 100);
            return discountPerItem * quantity;
        }
    }
}
