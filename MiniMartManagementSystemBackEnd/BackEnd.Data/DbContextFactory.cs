using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BackEnd.Data
{
    public class DbContextFactory : IDesignTimeDbContextFactory<DBContext>
    {
        public DBContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            Console.WriteLine(configuration.GetConnectionString("DefaultConnection"));
            var builder = new DbContextOptionsBuilder<DBContext>();
            builder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            return new DBContext(builder.Options);
        }
    }
}
