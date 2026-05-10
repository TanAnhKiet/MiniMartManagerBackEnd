using AutoMapper;
using BackEnd.Core.Domain.Entities;
using BackEnd.Core.Domain.Identity;
using BackEnd.Core.Models.Request;
using BackEnd.Core.Models.Response;
using BackEnd.Core.SeedWorks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MiniMartManagementAPI.Service.System
{
    public class SystemService : ISystemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public SystemService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // --- Cửa hàng ---
        public async Task<StoreResponseDTO> GetStoreById(Guid id)
        {
            var store = await _unitOfWork.StoreRepository.GetByIdAsync(id);
            return _mapper.Map<StoreResponseDTO>(store);
        }

        public async Task<StoreResponseDTO> UpdateStore(StoreRequestDTO request)
        {
            if (request.Id == null) return null!;
            var existingStore = await _unitOfWork.StoreRepository.GetByIdAsync(request.Id.Value, trackChanges: true);
            if (existingStore == null) return null!;

            _mapper.Map(request, existingStore);
            await _unitOfWork.StoreRepository.UpdateAsync(existingStore);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<StoreResponseDTO>(existingStore);
        }

        // --- Nhân viên ---
        public async Task<IEnumerable<EmployeeResponseDTO>> GetAllEmployees()
        {
            var employees = await _unitOfWork.EmployeeRepository.GetAllAsync();
            return _mapper.Map<List<EmployeeResponseDTO>>(employees);
        }

        private async Task<string> GenerateEmployeeCode()
        {
            int currentCount = await _unitOfWork.EmployeeRepository.CountAsync();
            string stt = (currentCount + 1).ToString().PadLeft(3, '0');
            string datePart = DateTime.Now.ToString("ddMMyy");
            return $"MNV{stt}{datePart}";
        }

        public async Task<EmployeeResponseDTO> CreateEmployee(EmployeeRequestDTO request)
        {
            string employeeCode = await GenerateEmployeeCode();
            var user = new AppUser { UserName = employeeCode, Id = Guid.NewGuid(), FullName = request.FullName };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded) throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            try
            {
                var employee = _mapper.Map<EmployeeEntity>(request);
                employee.EmployeeCode = employeeCode;
                employee.AppUserId = user.Id;
                employee.CreatedAt = DateTime.Now;

                _unitOfWork.EmployeeRepository.Add(employee);
                await _unitOfWork.CompleteAsync();
                return _mapper.Map<EmployeeResponseDTO>(employee);
            }
            catch
            {
                await _userManager.DeleteAsync(user);
                throw;
            }
        }

        public async Task<EmployeeResponseDTO> UpdateEmployee(EmployeeResponseDTO request)
        {
            if (request.Id == Guid.Empty) return null!;
            var existingEmployee = await _unitOfWork.EmployeeRepository.GetByIdAsync(request.Id, trackChanges: true);
            if (existingEmployee == null) return null!;

            _mapper.Map(request, existingEmployee);
            await _unitOfWork.EmployeeRepository.UpdateAsync(existingEmployee);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<EmployeeResponseDTO>(existingEmployee);
        }

        public async Task<bool> DeleteEmployee(Guid id)
        {
            var employee = await _unitOfWork.EmployeeRepository.GetByIdAsync(id, trackChanges: true);
            if (employee == null) return false;

            var user = await _userManager.FindByIdAsync(employee.AppUserId.ToString());
            if (user != null) await _userManager.DeleteAsync(user);

            _unitOfWork.EmployeeRepository.Remove(employee);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        // --- Tài khoản thanh toán ---
        public async Task<IEnumerable<StorePaymentAccountResponseDTO>> GetPaymentAccountsByStoreId(Guid storeId)
        {
            var accounts = await _unitOfWork.StorePaymentAccountRepository.GetAllByIdAsync(storeId);
            return _mapper.Map<List<StorePaymentAccountResponseDTO>>(accounts);
        }

        public async Task<StorePaymentAccountResponseDTO> CreatePaymentAccount(StorePaymentAccountRequestDTO request)
        {
            var account = _mapper.Map<StorePaymentAccountEntity>(request);
            account.CreatedAt = DateTime.UtcNow;
            _unitOfWork.StorePaymentAccountRepository.Add(account);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<StorePaymentAccountResponseDTO>(account);
        }

        public async Task<StorePaymentAccountResponseDTO> UpdatePaymentAccount(StorePaymentAccountRequestDTO request)
        {
            if (request.Id == null) return null!;
            var existingAccount = await _unitOfWork.StorePaymentAccountRepository.GetByIdAsync(request.Id.Value, trackChanges: true);
            if (existingAccount == null) return null!;

            _mapper.Map(request, existingAccount);
            await _unitOfWork.StorePaymentAccountRepository.UpdateAsync(existingAccount);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<StorePaymentAccountResponseDTO>(existingAccount);
        }

        public async Task<bool> DeletePaymentAccount(Guid id)
        {
            var account = await _unitOfWork.StorePaymentAccountRepository.GetByIdAsync(id, trackChanges: true);
            if (account == null) return false;

            _unitOfWork.StorePaymentAccountRepository.Remove(account);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        // --- Vai trò & Quyền hạn ---
        public async Task<IEnumerable<object>> GetAllRoles()
        {
            return await _roleManager.Roles.Select(r => new { r.Id, r.Name }).ToListAsync();
        }

        public async Task<bool> CreateRole(string roleName)
        {
            var result = await _roleManager.CreateAsync(new AppRole { Name = roleName, DisplayName = roleName });
            return result.Succeeded;
        }

        public async Task<IEnumerable<string>> GetPermissionsOfRole(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null) return null!;
            var claims = await _roleManager.GetClaimsAsync(role);
            return claims.Where(c => c.Type == "Permissions").Select(c => c.Value).ToList();
        }

        public async Task<bool> UpdatePermissionsOfRole(string roleName, List<string> permissions)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null) return false;

            var existingClaims = await _roleManager.GetClaimsAsync(role);
            foreach (var claim in existingClaims.Where(c => c.Type == "Permissions"))
            {
                await _roleManager.RemoveClaimAsync(role, claim);
            }

            foreach (var permission in permissions)
            {
                await _roleManager.AddClaimAsync(role, new Claim("Permissions", permission));
            }
            return true;
        }
    }
}
