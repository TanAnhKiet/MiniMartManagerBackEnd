using BackEnd.Core.Domain.Entities;
using BackEnd.Core.SeedWorks;

namespace BackEnd.Core.Repositories
{
    public interface ISupplierRepository : IRepository<SupplierEntity, Guid>
    {
        Task<SupplierEntity> UpdateAsync(SupplierEntity supplier);
    }
}
