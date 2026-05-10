using BackEnd.Core.Models.Request;
using BackEnd.Core.Models.Response;

namespace MiniMartManagementAPI.Service.System
{
    public interface ISystemService
    {
        // Cửa hàng
        Task<StoreResponseDTO> GetStoreById(Guid id);
        Task<StoreResponseDTO> UpdateStore(StoreRequestDTO request);

        // Nhân viên
        Task<IEnumerable<EmployeeResponseDTO>> GetAllEmployees();
        Task<EmployeeResponseDTO> CreateEmployee(EmployeeRequestDTO request);
        Task<EmployeeResponseDTO> UpdateEmployee(EmployeeResponseDTO request);
        Task<bool> DeleteEmployee(Guid id);

        // Tài khoản thanh toán
        Task<IEnumerable<StorePaymentAccountResponseDTO>> GetPaymentAccountsByStoreId(Guid storeId);
        Task<StorePaymentAccountResponseDTO> CreatePaymentAccount(StorePaymentAccountRequestDTO request);
        Task<StorePaymentAccountResponseDTO> UpdatePaymentAccount(StorePaymentAccountRequestDTO request);
        Task<bool> DeletePaymentAccount(Guid id);

        // Vai trò & Quyền hạn
        Task<IEnumerable<object>> GetAllRoles();
        Task<bool> CreateRole(string roleName);
        Task<IEnumerable<string>> GetPermissionsOfRole(string roleName);
        Task<bool> UpdatePermissionsOfRole(string roleName, List<string> permissions);
    }
}
