using BackEnd.Core.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace BackEnd.Core.SeedWorks
{
    // kiem tra xem cac thao tac tren database da duoc thuc hien thanh cong hay chua, neu thanh cong thi se commit cac thay doi vao database, neu khong thi se rollback lai
    public interface IUnitOfWork : IDisposable
    {
        Task<IDbContextTransaction> BeginTransactionAsync();
        IStoreRepository StoreRepository { get; }
        IProductRepository ProductRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IEmployeeRepository EmployeeRepository { get; }
        ISupplierRepository SupplierRepository { get; }
        IOrderRepository OrderRepository { get; }
        IImportRepository ImportRepository { get; }
        IImportItemRepository ImportItemRepository { get; }
        IOrderItemRepository OrderItemRepository { get; }
        IStorePaymentAccountRepository StorePaymentAccountRepository { get; }
        IPromotionRepository PromotionRepository { get; }
        Task<int> CompleteAsync();
    }
}
