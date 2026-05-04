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
            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "RootAdmin");
            Guid adminRoleId;

            if (adminRole == null)
            {
                Console.WriteLine(">>>> [SEEDING] Không thấy Role RootAdmin, đang tạo mới...");
                var newRole = new AppRole
                {
                    Id = Guid.NewGuid(),
                    Name = "RootAdmin",
                    NormalizedName = "ROOTADMIN",
                    DisplayName = "Chủ cửa hàng"
                };
                await context.Roles.AddAsync(newRole);
                await context.SaveChangesAsync();
                adminRoleId = newRole.Id;
            }
            else
            {
                adminRoleId = adminRole.Id;
                Console.WriteLine($">>>> [SEEDING] Đã có Role RootAdmin.");
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
                    RoleId = adminRoleId
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

            Console.WriteLine(">>>> [SEEDING] Quá trình Seeding kết thúc thành công.");
        }
    }
}