using BackEnd.Core.Domain.Entities;

namespace MiniMartManagementAPI.Service.Promotions
{
    public interface IPromotionStrategy
    {
        decimal CalculateDiscount(PromotionEntity promotion, decimal originalPrice, int quantity);
    }
}
