using System;
using System.Collections.Generic;
using System.Text;
using BackEnd.Core.Domain.Entities;
using BackEnd.Data.SeedWorks;
using BackEnd.Core.Repositories;
using AutoMapper;


namespace BackEnd.Data.Repositories
{
    public class StoreRepository : RepositoryBase<StoreEntity, Guid>, IStoreRepository
    {
        private readonly IMapper _mapper;
        public StoreRepository(DBContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public async Task<StoreEntity> UpdateAsync(StoreEntity entity)
        {
            _context.Set<StoreEntity>().Update(entity);
            return await Task.FromResult(entity);
        }
    }
}
