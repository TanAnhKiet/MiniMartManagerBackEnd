using AutoMapper;
using BackEnd.Core.Domain.Entities;
using BackEnd.Core.Domain.Identity;
using BackEnd.Core.Models;
using BackEnd.Core.Models.Request;
using BackEnd.Core.Models.Response;
using BackEnd.Core.Models.Function;
using BackEnd.Core.SeedWorks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MiniMartManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "RootAdmin, Manager")]
    public class EmployeeController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public EmployeeController(IUnitOfWork unitOfWork, IMapper mapper, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet("GetAllEmployee")]
        public async Task<ActionResult> GetAllEmployee()
        {
            var employees = await _unitOfWork.EmployeeRepository.GetAllAsync();

            if (User.IsInRole("Manager"))
            {
                var adminUsers = await _userManager.GetUsersInRoleAsync("RootAdmin");
                var adminIds = adminUsers.Select(u => u.Id).ToList();
                employees = employees.Where(e => !adminIds.Contains(e.AppUserId)).ToList();
            }

            var employeeDtos = _mapper.Map<List<EmployeeResponseDTO>>(employees);
            return Ok(employeeDtos);
        }

        private async Task<string> GenerateEmployeeCode()
        {
            int currentCount = await _unitOfWork.EmployeeRepository.CountAsync();
            string stt = (currentCount + 1).ToString().PadLeft(3, '0');
            string datePart = DateTime.Now.ToString("ddMMyy");
            return $"MNV{stt}{datePart}";
        }

        [HttpPost("create-employee")]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeRequestDTO request)
        {
            string employeeCode = await GenerateEmployeeCode();
            var user = new AppUser { UserName = employeeCode, Id = Guid.NewGuid(), FullName = request.FullName };
            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            try
            {
                var employee = _mapper.Map<EmployeeEntity>(request);
                employee.EmployeeCode = employeeCode;
                employee.AppUserId = user.Id;
                employee.CreatedAt = DateTime.Now;

                _unitOfWork.EmployeeRepository.Add(employee);
                var saveResult = await _unitOfWork.CompleteAsync();

                if (saveResult > 0)
                {
                    var response = _mapper.Map<EmployeeResponseDTO>(employee);
                    return Ok(response);
                }

                throw new Exception("Lưu Employee thất bại");
            }
            catch (Exception ex)
            {
                await _userManager.DeleteAsync(user);
                var innerError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, $"Lỗi: {innerError}");
            }
        }

        [HttpPut("UpdateEmployee")]
        public async Task<ActionResult> UpdateEmployee([FromBody] EmployeeResponseDTO request)
        {
            if (request == null || request.Id == Guid.Empty)
            {
                return BadRequest("Dữ liệu không hợp lệ");
            }

            var existingEmployee = await _unitOfWork.EmployeeRepository.GetByIdAsync(request.Id);
            if (existingEmployee == null)
            {
                return NotFound("Không tìm thấy nhân viên");
            }

            var targetUser = await _userManager.FindByIdAsync(existingEmployee.AppUserId.ToString());
            if (targetUser != null && await _userManager.IsInRoleAsync(targetUser, "RootAdmin") && User.IsInRole("Manager"))
            {
                return StatusCode(StatusCodes.Status403Forbidden, "Manager không thể thao tác lên tài khoản RootAdmin.");
            }

            _mapper.Map(request, existingEmployee);

            try
            {
                await _unitOfWork.EmployeeRepository.UpdateAsync(existingEmployee);
                await _unitOfWork.CompleteAsync();

                var resultDto = _mapper.Map<EmployeeResponseDTO>(existingEmployee);
                return Ok(resultDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi cập nhật: {ex.Message}");
            }
        }

        [HttpPost("UpdateEmployeeAccount")]
        public async Task<ActionResult> UpdateEmployeeAccount([FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var user = await _userManager.FindByIdAsync(request.AppUserId.ToString());
                if (user == null) return NotFound("Tài khoản không tồn tại.");

                bool isAdmin = User.IsInRole("RootAdmin") || User.IsInRole("Manager");
                IdentityResult result;

                if (isAdmin)
                {
                    await _userManager.RemovePasswordAsync(user);
                    result = await _userManager.AddPasswordAsync(user, request.NewPassword);
                }
                else
                {
                    if (string.IsNullOrEmpty(request.OldPassword))
                    {
                        return BadRequest("Nhân viên phải cung cấp mật khẩu cũ.");
                    }

                    var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (currentUserId != request.AppUserId.ToString())
                    {
                        return StatusCode(StatusCodes.Status403Forbidden, new
                        {
                            Message = "Bạn không có quyền đổi mật khẩu cho người khác."
                        });
                    }

                    result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
                }

                if (result.Succeeded)
                {
                    return Ok(new { Message = isAdmin ? "Admin đã reset mật khẩu thành công." : "Bạn đã đổi mật khẩu thành công." });
                }

                return BadRequest(new { Errors = result.Errors });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi hệ thống.", Detail = ex.Message });
            }
        }

        [HttpDelete("DeleteEmployee/{id}")]
        public async Task<ActionResult> DeleteEmployee(Guid id)
        {
            var employee = await _unitOfWork.EmployeeRepository.GetByIdAsync(id, trackChanges: true);
            if (employee == null)
            {
                return NotFound("Không tìm thấy nhân viên.");
            }

            var targetUser = await _userManager.FindByIdAsync(employee.AppUserId.ToString());
            if (targetUser != null && await _userManager.IsInRoleAsync(targetUser, "RootAdmin") && User.IsInRole("Manager"))
            {
                return StatusCode(StatusCodes.Status403Forbidden, "Manager không thể xóa tài khoản RootAdmin.");
            }

            var hasOrders = await _unitOfWork.OrderRepository.GetQueryable().AnyAsync(o => o.EmployeeId == id);
            if (hasOrders)
            {
                return BadRequest("Không thể xóa nhân viên này vì đã có dữ liệu hóa đơn liên quan. Để đảm bảo tính chính xác của báo cáo, vui lòng chuyển trạng thái nhân viên sang 'Ngưng hoạt động' thay vì xóa.");
            }

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(employee.AppUserId.ToString());
                    if (user != null)
                    {
                        var userResult = await _userManager.DeleteAsync(user);
                        if (!userResult.Succeeded)
                        {
                            return BadRequest("Không thể xóa tài khoản liên quan đến nhân viên này.");
                        }
                    }

                    _unitOfWork.EmployeeRepository.Remove(employee);
                    await _unitOfWork.CompleteAsync();

                    await transaction.CommitAsync();
                    return Ok(new { Message = "Đã xóa nhân viên và tài khoản thành công." });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, $"Lỗi hệ thống khi xóa: {ex.Message}");
                }
            }
        }
    }
}

