using BackEnd.Core.Domain.Entities;
using BackEnd.Core.SeedWorks;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackEnd.Core.Repositories
{
    public interface ICategoryRepository : IRepository<CategoryEntity, Guid>
    {
        Task<CategoryEntity> UpdateAsync(CategoryEntity category);
    }
}
