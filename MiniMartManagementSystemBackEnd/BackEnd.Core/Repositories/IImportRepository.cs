using BackEnd.Core.Domain.Entities;
using BackEnd.Core.SeedWorks;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackEnd.Core.Repositories
{
    public interface IImportRepository : IRepository<ImportEntity, Guid>
    {
        Task<ImportEntity> UpdateAsync(ImportEntity import);
    }
}
