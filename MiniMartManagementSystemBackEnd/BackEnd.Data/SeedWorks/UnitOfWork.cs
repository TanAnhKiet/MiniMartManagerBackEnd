using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using BackEnd.Core.Repositories;
using BackEnd.Core.SeedWorks;
using BackEnd.Data.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace BackEnd.Data.SeedWorks
{
    // Trien khai các phương thức để quản lý transaction, ví dụ như BeginTransaction, Commit, Rollback, v.v.
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DBContext _context; // DBContext sẽ được sử dụng để quản lý transaction, thuc hiện các thao tác với database, v.v.

        public UnitOfWork(DBContext context, IMapper mapper) // Dua DBContext vào UnitOfWork thông qua constructor
        {
            _context = context;
            ProductRepository = new ProductRepository(_context, mapper); // Khởi tạo repository cụ thể, ví dụ như ProductRepository, và truyền DBContext vào để quản lý các thao tác liên quan đến Product
            StoreRepository = new StoreRepository(_context, mapper); 
            CategoryRepository = new CategoryRepository(_context, mapper); 
            EmployeeRepository = new EmployeeRepository(_context, mapper); 
            SupplierRepository = new SupplierRepository(_context, mapper);
            OrderRepository = new OrderRepository(_context, mapper); 
            ImportRepository = new ImportRepository(_context, mapper); 
            OrderItemRepository = new OrderItemRepository(_context, mapper); 
            ImportItemRepository = new ImportItemRepository(_context, mapper); 
            StorePaymentAccountRepository = new StorePaymentAccountRepository(_context, mapper); 
            PromotionRepository = new PromotionRepository(_context, mapper);
        }
        public IStoreRepository StoreRepository { get;private set;} // Khai báo một repository cụ thể, ví dụ như StoreRepository, để quản lý các thao tác liên quan đến Store
        public IProductRepository ProductRepository { get; private set; } 
        public ICategoryRepository CategoryRepository { get; private set; }  
        public IEmployeeRepository EmployeeRepository { get; private set; } 
        public ISupplierRepository SupplierRepository { get; private set; } 
        public IOrderRepository OrderRepository { get; private set; } 
        public IImportRepository ImportRepository { get; private set; } 
        public IImportItemRepository ImportItemRepository { get; private set; }
        public IOrderItemRepository OrderItemRepository { get; private set; } 
        public IStorePaymentAccountRepository StorePaymentAccountRepository { get; private set; }
        public IPromotionRepository PromotionRepository { get; private set; }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task<int> CompleteAsync() // Phương thức này sẽ được gọi để lưu các thay đổi vào database, ví dụ như khi gọi SaveChangesAsync trên DBContext
        {
            return await _context.SaveChangesAsync();
        }
        public void Dispose() // Phương thức này sẽ được gọi để giải phóng tài nguyên, ví dụ như khi sử dụng UnitOfWork trong một using statement
        {
            _context.Dispose();
        }
    }
}
