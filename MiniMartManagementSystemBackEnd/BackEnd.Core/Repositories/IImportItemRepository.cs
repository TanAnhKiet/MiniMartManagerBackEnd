using BackEnd.Core.Domain.Entities;
using BackEnd.Core.SeedWorks;

namespace BackEnd.Core.Repositories
{
    public interface IImportItemRepository : IRepository<ImportItemEntity, Guid>
    {
        Task<ImportItemEntity> UpdateAsync(ImportItemEntity importItem);
    }
}
