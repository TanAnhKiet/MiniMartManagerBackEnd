using BackEnd.Core.Domain.Entities;

namespace MiniMartManagementAPI.Service.Promotions
{
    public class BuyXGetYPromotionStrategy : IPromotionStrategy
    {
        public decimal CalculateDiscount(PromotionEntity promotion, decimal originalPrice, int quantity)
        {
            if (promotion.BuyQuantity == null || promotion.GetQuantity == null || promotion.BuyQuantity <= 0) return 0;

            int buy = promotion.BuyQuantity.Value;
            int get = promotion.GetQuantity.Value;

            // Số lượng được tặng = (Số lượng mua / (X + Y)) * Y
            // Lưu ý: Logic này giả định khách hàng lấy cả phần được tặng vào giỏ hàng
            // Nếu khách hàng mua 3 (Mua 2 tặng 1), thì họ chỉ trả tiền cho 2.
            
            int freeItems = (quantity / (buy + get)) * get;
            return freeItems * originalPrice;
        }
    }
}
