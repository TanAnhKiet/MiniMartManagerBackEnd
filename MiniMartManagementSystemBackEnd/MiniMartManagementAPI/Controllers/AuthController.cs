using BackEnd.Core.Constants;
using BackEnd.Core.Domain.Identity;
using BackEnd.Core.Models.IdentityModels;
using BackEnd.Core.Models.System;
using BackEnd.Core.SeedWorks;
using BackEnd.Core.SeedWorks.Constans;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniMartManagementAPI.Extensions;
using MiniMartManagementAPI.Service;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;

namespace MiniMartManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AuthController> _logger;
        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, RoleManager<AppRole> roleManager, IUnitOfWork unitOfWork, ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthenticatedResult>> Login([FromBody] LoginRequest request)
        {

            // Authentication
            if (request == null)
            {
                return BadRequest("Invalid request");
            }

            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null || user.IsActive == false)
            {
                _logger.LogWarning("Login failed for user: {UserName} - User not found or inactive", request.UserName);
                return Unauthorized();
            }
            var result = await _signInManager.PasswordSignInAsync(request.UserName, request.Password, false, lockoutOnFailure: true);
            if (result.IsLockedOut)
            {
                _logger.LogWarning("Login failed for user: {UserName} - Account locked out", request.UserName);
                return Unauthorized();
            }
            if (!result.Succeeded)
            {
                _logger.LogWarning("Login failed for user: {UserName} - Invalid password", request.UserName);
                return Unauthorized();
            }

            _logger.LogInformation("Login successful for user: {UserName}", request.UserName);

            // 1. TRUY XUẤT THÔNG TIN NHÂN VIÊN ĐỂ LẤY StoreId
            // Giả sử trong bảng Employee của bạn có cột UserId để liên kết với IdentityUser
            
            var employee = await _unitOfWork.EmployeeRepository.GetQueryable()
            .Where(e => e.AppUserId == user.Id)
            .FirstOrDefaultAsync(); // Lúc này hàm này không cần tham số bên trong nữa

            if (employee == null)
            {
                return BadRequest("Tài khoản này chưa được liên kết với nhân viên nào.");
            }

            // 2. Authorization & Claims
            var role = await _userManager.GetRolesAsync(user);
            var permissions = await this.GetPermissionsByUserIdAsync(user.Id.ToString());

            var claims = new[] {
        new Claim(UserClaims.UserId, user.Id.ToString()),
        new Claim(UserClaims.EmployeeId, employee.Id.ToString()),
        new Claim(UserClaims.FullName, user.FullName),
        new Claim(UserClaims.Email, user.Email),
        new Claim(UserClaims.Role, string.Join(",", role)),
        new Claim(UserClaims.Permission, JsonSerializer.Serialize(permissions)),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        // Lấy StoreId thực tế từ bảng Employee
        new Claim("StoreId", employee.StoreId.ToString())
    };

            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);
            return Ok(new AuthenticatedResult
            {
                Token = accessToken,
                RefreshToken = refreshToken
            });
        }
        [HttpPost("logout")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirstValue(UserClaims.UserId);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = DateTime.MinValue;
                await _userManager.UpdateAsync(user);
                _logger.LogInformation("User logged out successfully: {UserId}", userId);
            }
            return Ok(new { message = "Logged out successfully" });
        }

        private async Task<List<string>> GetPermissionsByUserIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new List<string>(); // Check null cho chắc

            var roles = await _userManager.GetRolesAsync(user);
            var permissions = new List<string>();

            // Nếu là Admin thì lấy sạch quyền từ Class Constants
            if (roles.Contains(Roles.Admin))
            {
                var allPermissions = new List<RoleClaimsDTO>();
                var types = typeof(Permissions).GetTypeInfo().DeclaredNestedTypes;
                foreach (var type in types)
                {
                    allPermissions.GetPermission(type);
                }
                permissions.AddRange(allPermissions.Select(p => p.Value));
            }
            else
            {
                // Nếu là nhân viên thường thì lấy quyền từ DB (RoleClaims)
                foreach (var roleName in roles)
                {
                    var role = await _roleManager.FindByNameAsync(roleName);
                    if (role != null)
                    {
                        var claims = await _roleManager.GetClaimsAsync(role);
                        // Chỉ lấy những claim có Type là "Permissions"
                        permissions.AddRange(claims.Where(x => x.Type == "Permissions").Select(x => x.Value));
                    }
                }
            }
            return permissions.Distinct().ToList();
        }
    }
}
