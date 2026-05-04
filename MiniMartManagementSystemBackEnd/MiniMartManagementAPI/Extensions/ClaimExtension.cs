using BackEnd.Core.Domain.Identity;
using BackEnd.Core.Models.System;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.Reflection;
using System.Security.Claims;

namespace MiniMartManagementAPI.Extensions
{
    public static class ClaimExtension
    {
        // Thêm tham số allPermission vào để hàm có chỗ mà Add dữ liệu vào
        public static void GetPermission(this List<RoleClaimsDTO> allPermission, Type policy)
        {
            // Lấy tất cả các hằng số (Public + Static)
            FieldInfo[] fields = policy.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            foreach (FieldInfo field in fields)
            {
                // Lấy giá trị của hằng số (ví dụ: "Permissions.Sales.CreateOrder")
                string permissionValue = field.GetValue(null)?.ToString() ?? string.Empty;

                // Lấy Attribute Description
                var attribute = field.GetCustomAttribute<DescriptionAttribute>();

                // Nếu có Description thì dùng, không thì lấy tên Field
                string displayName = attribute != null ? attribute.Description : field.Name;

                // Add vào list kết quả
                allPermission.Add(new RoleClaimsDTO
                {
                    Value = permissionValue,
                    Type = "Permissions",
                    // DisplayName = displayName // Nếu DTO của bạn có trường này thì dùng nhé
                });
            }
        }
        public static async Task AddPermissionClaims(this RoleManager<AppRole> roleManager, AppRole appRole, string permission)
        {
            var claims = await roleManager.GetClaimsAsync(appRole);
            if (!claims.Any(a => a.Type == "Permissions" && a.Value == permission))
            {
                await roleManager.AddClaimAsync(appRole, new Claim("Permissions", permission));
            }
        }
    }
}