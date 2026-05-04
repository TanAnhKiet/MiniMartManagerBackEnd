using BackEnd.Core.Domain.Entities;
using BackEnd.Core.SeedWorks;
namespace BackEnd.Core.Repositories
{
    public interface IStoreRepository : IRepository<StoreEntity, Guid>
    {
        Task<StoreEntity> UpdateAsync(StoreEntity entity);
    }
}
