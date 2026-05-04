using BackEnd.Core.Constants;
using BackEnd.Core.Domain.Identity;
using BackEnd.Core.SeedWorks.Constans;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Reflection;
using System.Security.Claims;

namespace MiniMartManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Admin)] // Only RootAdmin can access this controller
    public class SystemSettingController : ControllerBase
    {
        private readonly RoleManager<AppRole> _roleManager;

        public SystemSettingController(RoleManager<AppRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return BadRequest("Tên vai trò không được để trống.");

            var roleExist = await _roleManager.RoleExistsAsync(roleName);
            if (roleExist)
                return BadRequest("Vai trò này đã tồn tại.");

            var result = await _roleManager.CreateAsync(new AppRole { Name = roleName, DisplayName = roleName });
            if (result.Succeeded)
                return Ok(new { Message = $"Đã tạo vai trò '{roleName}' thành công." });

            return BadRequest(result.Errors);
        }

        [HttpGet("GetAllRoles")]
        public IActionResult GetAllRoles()
        {
            var roles = _roleManager.Roles.Select(r => new { r.Id, r.Name }).ToList();
            return Ok(roles);
        }

        [HttpGet("GetPermissionsOfRole/{roleName}")]
        public async Task<IActionResult> GetPermissionsOfRole(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
                return NotFound("Không tìm thấy vai trò này.");

            var claims = await _roleManager.GetClaimsAsync(role);
            var permissions = claims.Where(c => c.Type == "Permissions").Select(c => c.Value).ToList();

            return Ok(permissions);
        }

        [HttpPost("UpdatePermissionsOfRole/{roleName}")]
        public async Task<IActionResult> UpdatePermissionsOfRole(string roleName, [FromBody] List<string> newPermissions)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
                return NotFound("Không tìm thấy vai trò này.");

            // Get existing claims
            var existingClaims = await _roleManager.GetClaimsAsync(role);
            var permissionClaims = existingClaims.Where(c => c.Type == "Permissions").ToList();

            // Remove all existing permission claims
            foreach (var claim in permissionClaims)
            {
                await _roleManager.RemoveClaimAsync(role, claim);
            }

            // Add new permissions
            foreach (var permission in newPermissions)
            {
                await _roleManager.AddClaimAsync(role, new Claim("Permissions", permission));
            }

            return Ok(new { Message = "Cập nhật quyền thành công." });
        }

        [HttpGet("GetAllAvailablePermissions")]
        public IActionResult GetAllAvailablePermissions()
        {
            var allPermissions = new List<object>();

            // Get all nested classes in Permissions class
            var groups = typeof(Permissions).GetNestedTypes(BindingFlags.Public | BindingFlags.Static);

            foreach (var group in groups)
            {
                var constants = group.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                                     .Where(fi => fi.IsLiteral && !fi.IsInitOnly);

                var groupPermissions = new List<object>();
                foreach (var constant in constants)
                {
                    var value = constant.GetRawConstantValue()?.ToString();
                    var descriptionAttribute = constant.GetCustomAttribute<DescriptionAttribute>();
                    var description = descriptionAttribute?.Description ?? value;

                    groupPermissions.Add(new { Key = value, Description = description });
                }

                allPermissions.Add(new
                {
                    GroupName = group.Name,
                    Permissions = groupPermissions
                });
            }

            return Ok(allPermissions);
        }
    }
}
