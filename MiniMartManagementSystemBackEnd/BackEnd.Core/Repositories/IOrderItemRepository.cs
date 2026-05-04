using BackEnd.Core.Domain.Entities;
using BackEnd.Core.SeedWorks;

namespace BackEnd.Core.Repositories
{
    public interface IOrderItemRepository : IRepository<OrderItemEntity, Guid>
    {
        Task<OrderItemEntity> UpdateAsync(OrderItemEntity orderItem);
    }
}
