using AutoMapper;
using BackEnd.Core.Domain.Entities;
using BackEnd.Core.Repositories;
using BackEnd.Data.SeedWorks;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackEnd.Data.Repositories
{
    class ProductRepository : RepositoryBase<ProductEntity, Guid>, IProductRepository
    {
        private readonly IMapper _mapper;
        public ProductRepository(DBContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public async Task<ProductEntity> UpdateAsync(ProductEntity product)
        {
            _context.Set<ProductEntity>().Update(product);
            return await Task.FromResult(product);
        }
    }
}
