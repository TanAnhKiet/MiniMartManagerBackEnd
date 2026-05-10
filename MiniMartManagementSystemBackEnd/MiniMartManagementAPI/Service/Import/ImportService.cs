using AutoMapper;
using BackEnd.Core.Domain.Entities;
using BackEnd.Core.Models.Request;
using BackEnd.Core.Models.Response;
using BackEnd.Core.SeedWorks;
using Microsoft.EntityFrameworkCore;

namespace MiniMartManagementAPI.Service.Import
{
    public class ImportService : IImportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ImportService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        private async Task<string> GenerateImportCode()
        {
            var today = DateTime.Now.Date;
            int countToday = await _unitOfWork.ImportRepository.GetQueryable()
                .CountAsync(i => i.CreatedAt.Date == today);
            return $"PN{today:yyyyMMdd}{(countToday + 1):D4}";
        }

        public async Task<ImportResponseDTO> CreateImport(ImportRequestDTO request)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var import = _mapper.Map<ImportEntity>(request);
                import.Id = Guid.NewGuid();
                import.ImportCode = await GenerateImportCode();
                import.CreatedAt = DateTime.Now;

                foreach (var itemRequest in request.Items)
                {
                    var product = await _unitOfWork.ProductRepository.GetByIdAsync(itemRequest.ProductId, trackChanges: true);
                    if (product == null) throw new Exception($"Sản phẩm {itemRequest.ProductId} không tồn tại.");

                    product.Quantity += itemRequest.Quantity;
                    await _unitOfWork.ProductRepository.UpdateAsync(product);
                }

                _unitOfWork.ImportRepository.Add(import);
                await _unitOfWork.CompleteAsync();
                await transaction.CommitAsync();

                return _mapper.Map<ImportResponseDTO>(import);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<ImportResponseDTO>> GetAllImports()
        {
            var imports = await _unitOfWork.ImportRepository.GetAllAsync();
            return _mapper.Map<List<ImportResponseDTO>>(imports);
        }

        public async Task<ImportResponseDTO> GetImportById(Guid id)
        {
            var import = await _unitOfWork.ImportRepository.GetQueryable()
                .Include(i => i.Supplier).Include(i => i.Employee).Include(i => i.Items).ThenInclude(ii => ii.Product)
                .FirstOrDefaultAsync(i => i.Id == id);
            return _mapper.Map<ImportResponseDTO>(import);
        }

        public async Task<IEnumerable<ImportResponseDTO>> GetImportsByDate(DateTime date)
        {
            var imports = _unitOfWork.ImportRepository.Find(i => i.CreatedAt.Date == date.Date);
            return _mapper.Map<IEnumerable<ImportResponseDTO>>(imports);
        }

        // --- Nhà cung cấp ---
        public async Task<IEnumerable<SupplierResponseDTO>> GetAllSuppliers()
        {
            var suppliers = await _unitOfWork.SupplierRepository.GetAllAsync();
            return _mapper.Map<List<SupplierResponseDTO>>(suppliers);
        }

        public async Task<SupplierResponseDTO> CreateSupplier(SupplierRequestDTO request)
        {
            var supplier = _mapper.Map<SupplierEntity>(request);
            supplier.Phone ??= "";
            supplier.Address ??= "";
            supplier.CreatedAt = DateTime.UtcNow;
            _unitOfWork.SupplierRepository.Add(supplier);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<SupplierResponseDTO>(supplier);
        }

        public async Task<SupplierResponseDTO> UpdateSupplier(SupplierRequestDTO request)
        {
            if (request.Id == null) return null!;
            var existingSupplier = await _unitOfWork.SupplierRepository.GetByIdAsync(request.Id.Value, trackChanges: true);
            if (existingSupplier == null) return null!;

            _mapper.Map(request, existingSupplier);
            await _unitOfWork.SupplierRepository.UpdateAsync(existingSupplier);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<SupplierResponseDTO>(existingSupplier);
        }

        public async Task<bool> DeleteSupplier(Guid id)
        {
            var supplier = await _unitOfWork.SupplierRepository.GetByIdAsync(id, trackChanges: true);
            if (supplier == null) return false;

            _unitOfWork.SupplierRepository.Remove(supplier);
            return await _unitOfWork.CompleteAsync() > 0;
        }
    }
}
