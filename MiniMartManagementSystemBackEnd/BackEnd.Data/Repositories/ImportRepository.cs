using AutoMapper;
using BackEnd.Core.Domain.Entities;
using BackEnd.Core.Repositories;
using BackEnd.Data.SeedWorks;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackEnd.Data.Repositories
{
    public class ImportRepository : RepositoryBase<ImportEntity, Guid>, IImportRepository
    { 
        private readonly IMapper _mapper;
        public ImportRepository(DBContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public async Task<ImportEntity> UpdateAsync(ImportEntity import)
        {
            _context.Set<ImportEntity>().Update(import);
            return await Task.FromResult(import);
        }
    }
}
