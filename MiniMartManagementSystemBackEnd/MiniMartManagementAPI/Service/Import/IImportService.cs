using BackEnd.Core.Models.Request;
using BackEnd.Core.Models.Response;

namespace MiniMartManagementAPI.Service.Import
{
    public interface IImportService
    {
        // Nhập hàng
        Task<ImportResponseDTO> CreateImport(ImportRequestDTO request);
        Task<IEnumerable<ImportResponseDTO>> GetAllImports();
        Task<ImportResponseDTO> GetImportById(Guid id);
        Task<IEnumerable<ImportResponseDTO>> GetImportsByDate(DateTime date);

        // Nhà cung cấp
        Task<IEnumerable<SupplierResponseDTO>> GetAllSuppliers();
        Task<SupplierResponseDTO> CreateSupplier(SupplierRequestDTO request);
        Task<SupplierResponseDTO> UpdateSupplier(SupplierRequestDTO request);
        Task<bool> DeleteSupplier(Guid id);
    }
}
