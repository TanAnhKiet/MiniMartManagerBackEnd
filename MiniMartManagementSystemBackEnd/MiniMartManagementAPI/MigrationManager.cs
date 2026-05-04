using BackEnd.Data;
using Microsoft.EntityFrameworkCore;

namespace MiniMartManagementAPI
{
    // Class này sẽ được gọi trong Program.cs để tự động migrate database khi ứng dụng khởi động
    public static class MigrationManager
    {
        // Phương thức mở rộng để migrate database
        public static WebApplication MigrateDatabase(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<DBContext>();
                try
                {
                    Console.WriteLine(">>>> [MIGRATION] Đang kiểm tra cấu trúc Database...");
                    context.Database.Migrate();

                    var configuration = services.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>();
                    // Gọi Seeding và đợi kết quả
                    new DBSeeding().SeedAsync(context, configuration).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("!!!! [LỖI] Có vấn đề khi Migrate hoặc Seeding:");
                    Console.WriteLine($"!!!! Chi tiết: {ex.Message}");
                    if (ex.InnerException != null)
                        Console.WriteLine($"!!!! Lỗi bên trong: {ex.InnerException.Message}");
                }
            }
            return app;
        }
    }
}
