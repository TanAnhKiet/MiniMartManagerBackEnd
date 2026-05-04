using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using BackEnd.Core.SeedWorks;
using BackEnd.Core.Models.System;
using Microsoft.EntityFrameworkCore;
namespace BackEnd.Data.SeedWorks
{
    // Trien khai các phương thức chung cho tất cả các repository, ví dụ như Add, Update, Delete, GetById, GetAll, v.v.
    public class RepositoryBase<T, Key> : IRepository<T, Key> where T : class
    {
        protected readonly DBContext _context;// DBContext là lớp đại diện cho phiên làm việc với cơ sở dữ liệu, nó quản lý các thực thể và thực hiện các thao tác truy vấn và lưu trữ dữ liệu.
        private readonly DbSet<T> _dbSet; // DbSet<T> là một tập hợp các thực thể của loại T, cu the la 1 bang trong cơ sở dữ liệu, nó cung cấp các phương thức để thực hiện các thao tác CRUD (Create, Read, Update, Delete) trên các thực thể đó.

        public RepositoryBase(DBContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>(); // Lấy DbSet<T> từ DBContext để thực hiện các thao tác trên bảng tương ứng trong cơ sở dữ liệu
        }
        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
           _dbSet.AddRange(entities);
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        public async Task<IEnumerable<T>> GetAllAsync(bool trackChanges = false)
        {
            return trackChanges ? await _dbSet.ToListAsync() : await _dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<T> GetByIdAsync(Key id, bool trackChanges = false)
        {
            return trackChanges ? await _dbSet.FindAsync(id) : await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => EF.Property<Key>(e, "Id").Equals(id));
        }

        public async Task<PagedResult<T>> GetPagedAsync(int pageIndex, int pageSize, Expression<Func<T, bool>>? predicate = null, bool trackChanges = false)
        {
            var query = trackChanges ? _dbSet.AsQueryable() : _dbSet.AsNoTracking();
            if (predicate != null) query = query.Where(predicate);

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public void Remove(T entity)
        {
             _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
             _dbSet.RemoveRange(entities);
        }
        public IQueryable<T> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }
    }
}
