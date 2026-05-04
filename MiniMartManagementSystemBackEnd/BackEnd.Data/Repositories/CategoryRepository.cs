using AutoMapper;
using BackEnd.Core.Domain.Entities;
using BackEnd.Core.Repositories;
using BackEnd.Data.SeedWorks;

namespace BackEnd.Data.Repositories
{
    public class CategoryRepository : RepositoryBase<CategoryEntity, Guid>, ICategoryRepository
    {
        private readonly IMapper _mapper;
        public CategoryRepository(DBContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public async Task<CategoryEntity> UpdateAsync(CategoryEntity category)
        {
            _context.Set<CategoryEntity>().Update(category);
            return await Task.FromResult(category);
        }
    }
}
