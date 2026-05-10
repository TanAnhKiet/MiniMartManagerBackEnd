using BackEnd.Core.Domain.Entities;

namespace MiniMartManagementAPI.Service.Promotions
{
    public class PromotionStrategyFactory
    {
        public static IPromotionStrategy GetStrategy(PromotionType type)
        {
            return type switch
            {
                PromotionType.PERCENT => new PercentPromotionStrategy(),
                PromotionType.FIXED => new FixedPromotionStrategy(),
                PromotionType.BUY_X_GET_Y => new BuyXGetYPromotionStrategy(),
                _ => throw new ArgumentOutOfRangeException(nameof(type), $"Loại khuyến mãi {type} không được hỗ trợ")
            };
        }
    }
}
