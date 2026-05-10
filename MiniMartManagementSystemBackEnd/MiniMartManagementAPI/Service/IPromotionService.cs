using BackEnd.Core.Domain.Entities;
using BackEnd.Core.Models;

namespace MiniMartManagementAPI.Service
{
    public interface IPromotionService
    {
        Task<IEnumerable<PromotionDTO>> GetAllPromotions(Guid storeId);
        Task<PromotionDTO> GetPromotionById(Guid id);
        Task<PromotionDTO> CreatePromotion(PromotionRequestDTO request);
        Task<PromotionDTO> UpdatePromotion(Guid id, PromotionRequestDTO request);
        Task<bool> DeletePromotion(Guid id);
        Task<decimal> CalculateDiscount(Guid storeId, PromotionScope scope, Guid? productId, decimal originalPrice, int quantity);
    }
}
