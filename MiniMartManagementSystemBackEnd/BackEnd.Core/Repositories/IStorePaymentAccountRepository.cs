using BackEnd.Core.Domain.Entities;
using BackEnd.Core.SeedWorks;

namespace BackEnd.Core.Repositories
{
    public interface IStorePaymentAccountRepository : IRepository<StorePaymentAccountEntity, Guid>
    {
        Task<StorePaymentAccountEntity> UpdateAsync(StorePaymentAccountEntity storePaymentAccount);
        Task<List<StorePaymentAccountEntity>> GetAllByIdAsync(Guid id);
        Task<StorePaymentAccountEntity?> GetByMerchantIdAsync(string merchantId);
    }
}
