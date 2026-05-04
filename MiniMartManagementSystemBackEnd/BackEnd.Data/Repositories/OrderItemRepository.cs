using AutoMapper;
using BackEnd.Core.Domain.Entities;
using BackEnd.Core.Repositories;
using BackEnd.Data.SeedWorks;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackEnd.Data.Repositories
{
    public class OrderItemRepository : RepositoryBase<OrderItemEntity, Guid>, IOrderItemRepository
    {
        private readonly IMapper _mapper;
        public OrderItemRepository(DBContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public async Task<OrderItemEntity> UpdateAsync(OrderItemEntity orderItem)
        {
            _context.Set<OrderItemEntity>().Update(orderItem);
            return await Task.FromResult(orderItem);
        }
    }
}
