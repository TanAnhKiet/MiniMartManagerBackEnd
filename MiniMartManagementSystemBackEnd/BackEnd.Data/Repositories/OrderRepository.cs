using AutoMapper;
using BackEnd.Core.Domain.Entities;
using BackEnd.Core.Repositories;
using BackEnd.Data.SeedWorks;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackEnd.Data.Repositories
{
    public class OrderRepository : RepositoryBase<OrderEntity, Guid>, IOrderRepository
    { 
        private readonly IMapper _mapper;   
        public OrderRepository(DBContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public async Task<OrderEntity> UpdateAsync(OrderEntity order)
        {
            _context.Set<OrderEntity>().Update(order);
            return await Task.FromResult(order);
        }
    }
}
