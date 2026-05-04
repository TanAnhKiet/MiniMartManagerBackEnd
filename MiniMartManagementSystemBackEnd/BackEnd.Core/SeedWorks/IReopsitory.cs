using System.Linq.Expressions;
using BackEnd.Core.Models.System;


namespace BackEnd.Core.SeedWorks
{
    // Chua cac thao tac co ban de lam viec voi cac doi tuong trong database, nhu lay du lieu, them moi, cap nhat, xoa, tim kiem
    public interface IRepository<T,Key> where T : class
    {
        Task<T> GetByIdAsync(Key id, bool trackChanges = false); // ham lay duoc su dung de lay mot doi tuong theo id
        Task<IEnumerable<T>> GetAllAsync(bool trackChanges = false); // ham lay duoc su dung de lay tat ca cac doi tuong
        Task<PagedResult<T>> GetPagedAsync(int pageIndex, int pageSize, Expression<Func<T, bool>>? predicate = null, bool trackChanges = false);

        IEnumerable<T> Find(Expression<Func<T, bool>> predicate); // ham lay duoc su dung de tim kiem cac doi tuong theo dieu kien

        void Add(T entity); // ham lay duoc su dung de them moi mot doi tuong
        void AddRange(IEnumerable<T> entities); // ham lay duoc su dung de them moi nhieu doi tuong
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities); // ham lay duoc su dung de xoa nhieu doi tuong
        IQueryable<T> GetQueryable();
    }
}
