using BackEnd.Core.Models.Request;
using BackEnd.Core.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniMartManagementAPI.Service.System;

namespace MiniMartManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SystemController : ControllerBase
    {
        private readonly ISystemService _systemService;

        public SystemController(ISystemService systemService)
        {
            _systemService = systemService;
        }

        // --- Cửa hàng ---
        [HttpGet("GetStoreById/{id}")]
        public async Task<IActionResult> GetStoreById(Guid id) => Ok(await _systemService.GetStoreById(id));

        [HttpPut("UpdateStore")]
        public async Task<IActionResult> UpdateStore([FromBody] StoreRequestDTO request) => Ok(await _systemService.UpdateStore(request));

        // --- Nhân viên ---
        [HttpGet("GetAllEmployees")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetAllEmployees() => Ok(await _systemService.GetAllEmployees());

        [HttpPost("CreateEmployee")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeRequestDTO request) 
            => Ok(await _systemService.CreateEmployee(request));

        [HttpPut("UpdateEmployee")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdateEmployee([FromBody] EmployeeResponseDTO request) 
            => Ok(await _systemService.UpdateEmployee(request));

        [HttpDelete("DeleteEmployee/{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteEmployee(Guid id)
        {
            var result = await _systemService.DeleteEmployee(id);
            if (!result) return NotFound();
            return Ok(new { Message = "Đã xóa nhân viên thành công" });
        }

        // --- Tài khoản thanh toán ---
        [HttpGet("GetPaymentAccounts/{storeId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetPaymentAccounts(Guid storeId) => Ok(await _systemService.GetPaymentAccountsByStoreId(storeId));

        [HttpPost("CreatePaymentAccount")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> CreatePaymentAccount([FromBody] StorePaymentAccountRequestDTO request) 
            => Ok(await _systemService.CreatePaymentAccount(request));

        [HttpPut("UpdatePaymentAccount")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdatePaymentAccount([FromBody] StorePaymentAccountRequestDTO request) 
            => Ok(await _systemService.UpdatePaymentAccount(request));

        [HttpDelete("DeletePaymentAccount/{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeletePaymentAccount(Guid id)
        {
            var result = await _systemService.DeletePaymentAccount(id);
            if (!result) return NotFound();
            return NoContent();
        }

        // --- Vai trò & Quyền hạn ---
        [HttpGet("GetAllRoles")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetAllRoles() => Ok(await _systemService.GetAllRoles());

        [HttpPost("CreateRole")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> CreateRole([FromBody] string roleName) 
            => Ok(await _systemService.CreateRole(roleName));

        [HttpGet("GetPermissions/{roleName}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetPermissions(string roleName) => Ok(await _systemService.GetPermissionsOfRole(roleName));

        [HttpPost("UpdatePermissions/{roleName}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdatePermissions(string roleName, [FromBody] List<string> permissions) 
            => Ok(await _systemService.UpdatePermissionsOfRole(roleName, permissions));
    }
}
