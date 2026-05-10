using BackEnd.Core.Domain.Entities;
using BackEnd.Core.SeedWorks;

namespace BackEnd.Core.Repositories
{
    public interface IPromotionRepository : IRepository<PromotionEntity, Guid>
    {
        Task<PromotionEntity> UpdateAsync(PromotionEntity promotion);
        Task<IEnumerable<PromotionEntity>> GetActivePromotions(Guid storeId, PromotionScope scope);
    }
}
