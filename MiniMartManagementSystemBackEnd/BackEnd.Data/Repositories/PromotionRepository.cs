using AutoMapper;
using BackEnd.Core.Domain.Entities;
using BackEnd.Core.Repositories;
using BackEnd.Data.SeedWorks;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Data.Repositories
{
    public class PromotionRepository : RepositoryBase<PromotionEntity, Guid>, IPromotionRepository
    {
        private readonly IMapper _mapper;
        public PromotionRepository(DBContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public async Task<PromotionEntity> UpdateAsync(PromotionEntity promotion)
        {
            _context.Set<PromotionEntity>().Update(promotion);
            return await Task.FromResult(promotion);
        }

        public async Task<IEnumerable<PromotionEntity>> GetActivePromotions(Guid storeId, PromotionScope scope)
        {
            var now = DateTime.UtcNow;
            return await _context.Promotions
                .Where(p => p.StoreId == storeId && 
                            p.Scope == scope && 
                            p.Status == PromotionStatus.ACTIVE &&
                            p.StartDate <= now && 
                            p.EndDate >= now)
                .ToListAsync();
        }
    }
}
