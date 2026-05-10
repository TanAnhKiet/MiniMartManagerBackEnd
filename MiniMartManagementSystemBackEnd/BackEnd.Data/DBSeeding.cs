using BackEnd.Core.Domain.Entities;
using BackEnd.Core.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BackEnd.Data
{
    public class DBSeeding
    {
        public async Task SeedAsync(DBContext context, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            Console.WriteLine(">>>> [SEEDING] Bắt đầu kiểm tra dữ liệu mẫu...");

            // --- 1. SEED STORE ---
            var store = await context.Stores.FirstOrDefaultAsync();
            Guid currentStoreId;

            if (store == null)
            {
                Console.WriteLine(">>>> [SEEDING] Không thấy Store nào, đang tạo Store mặc định...");
                var newStore = new StoreEntity
                {
                    Id = Guid.NewGuid(),
                    Name = "Chut chut Store",
                    Address = "Hà Nội",
                    Phone = "0123456789",
                    Email = "ChutChut99@gmail.com",
                    CreatedAt = DateTime.UtcNow
                };
                await context.Stores.AddAsync(newStore);
                await context.SaveChangesAsync();
                currentStoreId = newStore.Id;
            }
            else
            {
                currentStoreId = store.Id;
                Console.WriteLine($">>>> [SEEDING] Đã có Store: {store.Name} ({currentStoreId})");
            }

            // --- 2. SEED ROLES ---
            var managerRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Manager");
            Guid managerRoleId;

            if (managerRole == null)
            {
                Console.WriteLine(">>>> [SEEDING] Không thấy Role Manager, đang tạo mới...");
                var newRole = new AppRole
                {
                    Id = Guid.NewGuid(),
                    Name = "Manager",
                    NormalizedName = "MANAGER",
                    DisplayName = "Quản lý"
                };
                await context.Roles.AddAsync(newRole);
                await context.SaveChangesAsync();
                managerRoleId = newRole.Id;
            }
            else
            {
                managerRoleId = managerRole.Id;
                Console.WriteLine($">>>> [SEEDING] Đã có Role Manager.");
            }

            var staffRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Staff");
            if (staffRole == null)
            {
                await context.Roles.AddAsync(new AppRole
                {
                    Id = Guid.NewGuid(),
                    Name = "Staff",
                    NormalizedName = "STAFF",
                    DisplayName = "Nhân viên"
                });
                await context.SaveChangesAsync();
            }

            // --- 3. SEED USER & EMPLOYEE ---
            // Kiểm tra xem đã có user admin chưa
            var adminUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == "admin");

            if (adminUser == null)
            {
                Console.WriteLine(">>>> [SEEDING] Không thấy User admin, bắt đầu tạo User và Employee...");

                var userId = Guid.NewGuid();
                var passwordHasher = new PasswordHasher<AppUser>();

                var rootUser = new AppUser
                {
                    Id = userId,
                    FullName = "Tân Anh Kiệt",
                    UserName = "admin",
                    NormalizedUserName = "ADMIN",
                    Email = "kiettan17@gmail.com",
                    NormalizedEmail = "KIETTAN17@GMAIL.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                rootUser.PasswordHash = passwordHasher.HashPassword(rootUser, "Admin@123");
                await context.Users.AddAsync(rootUser);

                // Gán quyền cho User (UserRoles)
                await context.UserRoles.AddAsync(new IdentityUserRole<Guid>
                {
                    UserId = userId,
                    RoleId = managerRoleId
                });

                // Tạo Employee liên kết (Kiểm tra thêm lần nữa cho chắc chắn không trùng Employee)
                var hasEmployee = await context.Employees.AnyAsync(e => e.AppUserId == userId);
                if (!hasEmployee)
                {
                    var employee = new EmployeeEntity
                    {
                        Id = Guid.NewGuid(),
                        EmployeeCode = "NV001",
                        FullName = "Tân Anh Kiệt",
                        Address = "Hà Nội",
                        PhoneNumber = "0123456789",
                        DateOfBirth = new DateTime(1999, 2, 13),
                        AppUserId = userId,
                        StoreId = currentStoreId, // Dùng ID lấy từ bước 1
                        Position = "Admin",
                        CreatedAt = DateTime.UtcNow
                    };
                    await context.Employees.AddAsync(employee);
                }

                await context.SaveChangesAsync();
                Console.WriteLine(">>>> [SEEDING] Hoàn tất tạo User admin và Employee tương ứng!");
            }
            else
            {
                Console.WriteLine(">>>> [SEEDING] User admin đã tồn tại.");
            }

            // --- 4. SEED STORE PAYMENT ACCOUNT ---
            var hasPaymentAccount = await context.StorePaymentAccounts.AnyAsync();
            if (!hasPaymentAccount && currentStoreId != Guid.Empty)
            {
                Console.WriteLine(">>>> [SEEDING] Không thấy tài khoản thanh toán nào, đang tạo VNPay mặc định từ config...");
                
                var vnpayConfig = configuration.GetSection("Vnpay");
                if (vnpayConfig.Exists())
                {
                    var paymentAccount = new StorePaymentAccountEntity
                    {
                        Id = Guid.NewGuid(),
                        StoreId = currentStoreId,
                        Provider = "VNPay",
                        AccountName = "Cấu hình VNPay mặc định",
                        MerchantId = vnpayConfig["TmnCode"] ?? "TKD12CJH",
                        SecretKey = vnpayConfig["HashSecret"] ?? "RAPEX52LN0Q1L25OV5G9G29KG8B627IF",
                        PublicKey = "",
                        BaseUrl = vnpayConfig["BaseUrl"] ?? "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html",
                        ReturnUrl = vnpayConfig["ReturnUrl"] ?? "https://localhost:7289/api/Payment/PaymentCallback",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    await context.StorePaymentAccounts.AddAsync(paymentAccount);
                    await context.SaveChangesAsync();
                    Console.WriteLine(">>>> [SEEDING] Hoàn tất tạo tài khoản VNPay mặc định!");
                }
                else
                {
                    Console.WriteLine(">>>> [SEEDING] CẢNH BÁO: Không tìm thấy cấu hình VNPay trong appsettings.json để seed.");
                }
            }

            // --- 5. SEED CATEGORIES & PRODUCTS ---
            var hasCategories = await context.Categories.AnyAsync();
            if (!hasCategories)
            {
                Console.WriteLine(">>>> [SEEDING] Không thấy danh mục nào, đang tạo mặc định...");
                var categories = new List<CategoryEntity>
                {
                    new CategoryEntity { Id = Guid.NewGuid(), Name = "Sữa & Chế phẩm từ sữa", CreatedAt = DateTime.UtcNow },
                    new CategoryEntity { Id = Guid.NewGuid(), Name = "Đồ uống & Nước giải khát", CreatedAt = DateTime.UtcNow },
                    new CategoryEntity { Id = Guid.NewGuid(), Name = "Bánh kẹo", CreatedAt = DateTime.UtcNow },
                    new CategoryEntity { Id = Guid.NewGuid(), Name = "Chăm sóc cá nhân", CreatedAt = DateTime.UtcNow },
                    new CategoryEntity { Id = Guid.NewGuid(), Name = "Gia vị & Thực phẩm khô", CreatedAt = DateTime.UtcNow }
                };
                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();

                var hasProducts = await context.Products.AnyAsync();
                if (!hasProducts)
                {
                    Console.WriteLine(">>>> [SEEDING] Không thấy sản phẩm nào, đang tạo mẫu...");
                    var products = new List<ProductEntity>
                    {
                        new ProductEntity { Name = "Sữa Tươi Vinamilk 1L", Barcode = "8934563102001", CategoryId = categories[0].Id, SellPrice = 35000, Quantity = 100, WarningThreshold = 10, StoreId = currentStoreId, CreatedAt = DateTime.UtcNow },
                        new ProductEntity { Name = "Nước Ngọt Coca-Cola 330ml", Barcode = "8934563102002", CategoryId = categories[1].Id, SellPrice = 12000, Quantity = 250, WarningThreshold = 20, StoreId = currentStoreId, CreatedAt = DateTime.UtcNow },
                        new ProductEntity { Name = "Bánh Quy ChocoPie", Barcode = "8934563102003", CategoryId = categories[2].Id, SellPrice = 55000, Quantity = 80, WarningThreshold = 5, StoreId = currentStoreId, CreatedAt = DateTime.UtcNow },
                        new ProductEntity { Name = "Dầu Gội Clear Men 170g", Barcode = "8934563102004", CategoryId = categories[3].Id, SellPrice = 62000, Quantity = 45, WarningThreshold = 5, StoreId = currentStoreId, CreatedAt = DateTime.UtcNow },
                        new ProductEntity { Name = "Dầu Ăn Neptune 1L", Barcode = "8934563102005", CategoryId = categories[4].Id, SellPrice = 48000, Quantity = 120, WarningThreshold = 15, StoreId = currentStoreId, CreatedAt = DateTime.UtcNow }
                    };
                    await context.Products.AddRangeAsync(products);
                    await context.SaveChangesAsync();
                }
            }

            // --- 6. SEED PROMOTIONS ---
            var hasPromotions = await context.Promotions.AnyAsync();
            if (!hasPromotions && currentStoreId != Guid.Empty)
            {
                Console.WriteLine(">>>> [SEEDING] Không thấy khuyến mãi nào, đang tạo mẫu...");
                
                // Lấy sản phẩm đầu tiên nếu có
                var product = await context.Products.FirstOrDefaultAsync();
                
                var promotions = new List<PromotionEntity>
                {
                    new PromotionEntity
                    {
                        Id = Guid.NewGuid(),
                        Name = "Giảm giá khai trương 10%",
                        Type = PromotionType.PERCENT,
                        DiscountValue = 10,
                        Scope = PromotionScope.BAN_HANG,
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddMonths(1),
                        Status = PromotionStatus.ACTIVE,
                        StoreId = currentStoreId,
                        CreatedAt = DateTime.UtcNow,
                        ProductId = product?.Id // Áp dụng cho sản phẩm đầu tiên hoặc null (toàn bộ)
                    },
                    new PromotionEntity
                    {
                        Id = Guid.NewGuid(),
                        Name = "Mua 2 tặng 1",
                        Type = PromotionType.BUY_X_GET_Y,
                        BuyQuantity = 2,
                        GetQuantity = 1,
                        Scope = PromotionScope.BAN_HANG,
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddMonths(1),
                        Status = PromotionStatus.ACTIVE,
                        StoreId = currentStoreId,
                        CreatedAt = DateTime.UtcNow,
                        ProductId = product?.Id
                    }
                };
                
                await context.Promotions.AddRangeAsync(promotions);
                await context.SaveChangesAsync();
                Console.WriteLine(">>>> [SEEDING] Hoàn tất tạo khuyến mãi mẫu!");
            }

            Console.WriteLine(">>>> [SEEDING] Quá trình Seeding kết thúc thành công.");
        }
    }
}