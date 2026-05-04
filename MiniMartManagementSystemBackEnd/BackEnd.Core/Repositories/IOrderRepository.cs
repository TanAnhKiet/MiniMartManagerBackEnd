using BackEnd.Core.Domain.Entities;
using BackEnd.Core.SeedWorks;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackEnd.Core.Repositories
{
    public interface IOrderRepository : IRepository<OrderEntity, Guid>
    {
        Task<OrderEntity> UpdateAsync(OrderEntity order);
    }
}
