using AutoMapper;
using BackEnd.Core.Domain.Entities;
using BackEnd.Core.Repositories;
using BackEnd.Data.SeedWorks;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackEnd.Data.Repositories
{
    public class ImportItemRepository : RepositoryBase<ImportItemEntity, Guid>, IImportItemRepository
    {
        private readonly IMapper _mapper;
        public ImportItemRepository(DBContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public async Task<ImportItemEntity> UpdateAsync(ImportItemEntity importItem)
        {
            _context.Set<ImportItemEntity>().Update(importItem);
            return await Task.FromResult(importItem);
        }
    }
}
