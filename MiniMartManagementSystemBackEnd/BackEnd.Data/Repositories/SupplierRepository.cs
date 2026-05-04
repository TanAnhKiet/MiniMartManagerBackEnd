using AutoMapper;
using BackEnd.Core.Domain.Entities;
using BackEnd.Core.Repositories;
using BackEnd.Data.SeedWorks;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackEnd.Data.Repositories
{
    public class SupplierRepository : RepositoryBase<SupplierEntity, Guid>, ISupplierRepository
    {
        private readonly IMapper _mapper;
        public SupplierRepository(DBContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public async Task<SupplierEntity> UpdateAsync(SupplierEntity supplier)
        {
            _context.Set<SupplierEntity>().Update(supplier);
            return await Task.FromResult(supplier);
        }
    }
}

