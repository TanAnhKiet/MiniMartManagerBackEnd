using BackEnd.Core.Domain.Entities;
using BackEnd.Core.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
namespace BackEnd.Data
{
    // Class DBContext kế thừa từ IdentityDbContext để tích hợp các thực thể liên quan đến Identity như AppUser và AppRole
    // Su dung de quan ly va thuc hien cac thao tac lien quan den database, bao gom ca cac thuc the lien quan den Identity nhu AppUser va AppRole
    public class DBContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public DBContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<EmployeeEntity> Employees { get; set; } = null!;
        public DbSet<StoreEntity> Stores { get; set; } = null!;
        public DbSet<OrderEntity> Orders { get; set; } = null!;
        public DbSet<ProductEntity> Products { get; set; } = null!;
        public DbSet<CategoryEntity> Categories { get; set; } = null!;
        public DbSet<ImportEntity> Imports { get; set; } = null!;
        public DbSet<ImportItemEntity> ImportsItems { get; set; }
        public DbSet<OrderItemEntity> OrderItems { get; set; }
        public DbSet<SupplierEntity> Suppliers { get; set; }
        public DbSet<StorePaymentAccountEntity> StorePaymentAccounts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //Fluent API để cấu hình bảng và khóa chính cho các thực thể liên quan đến Identity
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("AppUserClaim").HasKey(x => x.Id); // Định nghĩa bảng AppUserClaim và khóa chính là Id
            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("AppRoleClaim").HasKey(x => x.Id);// Định nghĩa bảng AppRoleClaim và khóa chính là Id
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("AppUserLogin").HasKey(x => x.UserId);// Định nghĩa bảng AppUserLogin và khóa chính là UserId
            builder.Entity<IdentityUserToken<Guid>>().ToTable("AppUserToken").HasKey(x => new { x.UserId });// Định nghĩa bảng AppUserToken và khóa chính là UserId
            builder.Entity<IdentityUserRole<Guid>>().ToTable("AppUserRole").HasKey(x => new { x.UserId, x.RoleId });//  Định nghĩa bảng AppUserRole và khóa chính là tổ hợp UserId và RoleId để đảm bảo mỗi người dùng chỉ có một vai trò duy nhất
            var foreignKeys = builder.Model.GetEntityTypes()
            .SelectMany(e => e.GetForeignKeys());

            foreach (var relationship in foreignKeys)
            {
                relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;
                // ClientSetNull hoặc Restrict sẽ giúp vượt qua lỗi "multiple cascade paths"
            }
            // cấu hình tiền tệ mặc định cho các cột có kiểu dữ liệu decimal
            foreach (var property in builder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,2)");
            }
        }
    }
}
