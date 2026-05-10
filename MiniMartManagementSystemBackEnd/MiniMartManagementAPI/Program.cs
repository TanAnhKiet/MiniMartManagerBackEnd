using BackEnd.Core.ConfigOption;
using BackEnd.Core.Domain.Identity;
using BackEnd.Core.Models;
using BackEnd.Core.Models.Request;
using BackEnd.Core.Models.Response;
using BackEnd.Core.Models.Function;
using BackEnd.Core.SeedWorks;
using BackEnd.Data;
using BackEnd.Data.Repositories;
using BackEnd.Data.SeedWorks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using MiniMartManagementAPI;
using MiniMartManagementAPI.Service;
using MiniMartManagementAPI.SinaglR;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using Microsoft.AspNetCore.HttpOverrides;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/audit-log-.txt", rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
var configuration = builder.Configuration;
var connectionString= configuration.GetConnectionString("DefaultConnection");
// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IVNPayService, VNPayService>();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<MiniMartManagementAPI.Filtter.AuditLogFilter>();
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhostDev", policy =>
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// Sau đó sử dụng đúng Policy này
//app.UseCors("AllowSpecificOrigin");
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
    // Ngrok headers are trusted
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});
// Add db context and identity services
builder.Services.AddDbContext<DBContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;//yêu cầu ít nhất một chữ số
    options.Password.RequireLowercase = true;//yêu cầu ít nhất một chữ thường
    options.Password.RequireNonAlphanumeric = true;//yêu cầu ít nhất một ký tự đặc biệt
    options.Password.RequireUppercase = true;//yêu cầu ít nhất một chữ hoa
    options.Password.RequiredLength = 6;//yêu cầu độ dài tối thiểu của mật khẩu
    options.Password.RequiredUniqueChars = 1;//yêu cầu số lượng ký tự duy nhất trong mật khẩu
    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // thời gian khóa tài khoản sau khi đạt đến số lần đăng nhập thất bại tối đa
    options.Lockout.MaxFailedAccessAttempts = 5;// số lần đăng nhập thất bại tối đa trước khi tài khoản bị khóa
    options.Lockout.AllowedForNewUsers = true;// cho phép khóa tài khoản cho người dùng mới
    // User settings
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";//các ký tự được phép trong tên người dùng
    options.User.RequireUniqueEmail = false;// yêu cầu email phải là duy nhất cho mỗi người dùng
});

// Them Repository va UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<,>), typeof(RepositoryBase<,>));

// Them cac service nghiep vu va repository
// Tự động đăng ký các lớp Repository vào DI container
// Quét tất cả các Repository cụ thể (ví dụ StoreRepository)
var repositoryTypes = typeof(StoreRepository).Assembly.GetTypes()
    .Where(t => t.IsClass
           && !t.IsAbstract
           && !t.IsGenericType // <--- THÊM DÒNG NÀY để bỏ qua RepositoryBase
           && t.GetInterfaces().Any(i => i.Name.EndsWith("Repository")));

foreach (var type in repositoryTypes)
{
    var interfaceType = type.GetInterfaces().FirstOrDefault(i => i.Name.EndsWith("Repository"));
    if (interfaceType != null)
    {
        builder.Services.AddScoped(interfaceType, type);
    }
}
// them AutoMapper
builder.Services.AddAutoMapper(cfg =>
{
    // Request Profiles
    cfg.AddProfile<CategoryRequestDTO.MappingProfile>();
    cfg.AddProfile<ProductRequestDTO.MappingProfile>();
    cfg.AddProfile<EmployeeRequestDTO.MappingProfile>();
    cfg.AddProfile<ImportRequestDTO.MappingProfile>();
    cfg.AddProfile<OrderRequestDTO.MappingProfile>();
    cfg.AddProfile<StoreRequestDTO.MappingProfile>();
    cfg.AddProfile<StorePaymentAccountRequestDTO.MappingProfile>();
    cfg.AddProfile<SupplierRequestDTO.MappingProfile>();

    // Response Profiles
    cfg.AddProfile<CategoryResponseDTO.MappingProfile>();
    cfg.AddProfile<ProductResponseDTO.MappingProfile>();
    cfg.AddProfile<EmployeeResponseDTO.MappingProfile>();
    cfg.AddProfile<ImportResponseDTO.MappingProfile>();
    cfg.AddProfile<ImportItemResponseDTO.MappingProfile>();
    cfg.AddProfile<OrderResponseDTO.MappingProfile>();
    cfg.AddProfile<OrderItemResponseDTO.MappingProfile>();
    cfg.AddProfile<StoreResponseDTO.MappingProfile>();
    cfg.AddProfile<StorePaymentAccountResponseDTO.MappingProfile>();
    cfg.AddProfile<SupplierResponseDTO.MappingProfile>();
    cfg.AddProfile<PromotionDTO.MappingProfile>();
    cfg.AddProfile<PromotionRequestDTO.MappingProfile>();
});

builder.Services.AddScoped<MiniMartManagementAPI.Service.Promotion.IPromotionService, MiniMartManagementAPI.Service.Promotion.PromotionService>();
builder.Services.AddScoped<MiniMartManagementAPI.Service.POS.IPOSService, MiniMartManagementAPI.Service.POS.POSService>();
builder.Services.AddScoped<MiniMartManagementAPI.Service.Inventory.IInventoryService, MiniMartManagementAPI.Service.Inventory.InventoryService>();
builder.Services.AddScoped<MiniMartManagementAPI.Service.Import.IImportService, MiniMartManagementAPI.Service.Import.ImportService>();
builder.Services.AddScoped<MiniMartManagementAPI.Service.System.ISystemService, MiniMartManagementAPI.Service.System.SystemService>();
builder.Services.AddScoped<MiniMartManagementAPI.Service.Report.IReportService, MiniMartManagementAPI.Service.Report.ReportService>();

// Đăng ký ITokenService vào DI container
builder.Services.Configure<JwtTokenSettings>(configuration.GetSection("JwtTokenSettings"));
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IOptions<JwtTokenSettings>>().Value);
// Đăng ký AuthenticationService vào DI container
// Đăng ký Identity một cách chính thống
builder.Services.AddIdentity<AppUser, AppRole>()
    .AddEntityFrameworkStores<DBContext>() // Đây là dòng giải quyết lỗi IUserStore
    .AddDefaultTokenProviders();
builder.Services.AddScoped<ITokenService, TokenService>();

// Dang ky RealTime Service
builder.Services.AddSignalR();

// Them Authentication
var jwtSettings = new JwtTokenSettings();
configuration.GetSection("JwtTokenSettings").Bind(jwtSettings);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings?.Issuer ?? "MiniMartManagement",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.SecretKey ?? "U0hBLTI1Ni1TZWN1cmUtS2V5LUZvci1NaW5pTWFydC1NYW5hZ2VtZW50LTIwMjY=")),
        ClockSkew = TimeSpan.FromMinutes(5)
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Add("Token-Expired", "true");
            }
            Log.Error("Authentication failed: {Message}", context.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Log.Information("Token validated successfully for user: {User}", context.Principal?.Identity?.Name);
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddAuthorization();
//=================================================================================================================================
var app = builder.Build();
// Configure the HTTP request pipeline.
app.UseForwardedHeaders();
app.UseCors("AllowLocalhostDev");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sale Management API v1");
    });      
}


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
// seed data
app.MigrateDatabase();

app.MapHub<InventoryHub>("/InventoryHub");

app.Run();
