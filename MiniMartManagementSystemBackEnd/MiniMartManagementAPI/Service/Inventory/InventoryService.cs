using AutoMapper;
using BackEnd.Core.Domain.Entities;
using BackEnd.Core.Models.Request;
using BackEnd.Core.Models.Response;
using BackEnd.Core.Models.System;
using BackEnd.Core.SeedWorks;
using Microsoft.EntityFrameworkCore;

namespace MiniMartManagementAPI.Service.Inventory
{
    public class InventoryService : IInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public InventoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // --- Sản phẩm ---
        public async Task<IEnumerable<ProductResponseDTO>> GetAllProducts()
        {
            var productEntities = await _unitOfWork.ProductRepository.GetAllAsync();
            return _mapper.Map<List<ProductResponseDTO>>(productEntities);
        }

        public async Task<PagedResult<ProductResponseDTO>> GetProductsPaged(int pageIndex, int pageSize)
        {
            var pagedResult = await _unitOfWork.ProductRepository.GetPagedAsync(pageIndex, pageSize);
            return new PagedResult<ProductResponseDTO>
            {
                Items = _mapper.Map<List<ProductResponseDTO>>(pagedResult.Items),
                TotalCount = pagedResult.TotalCount,
                PageIndex = pagedResult.PageIndex,
                PageSize = pagedResult.PageSize
            };
        }

        public async Task<ProductResponseDTO> CreateProduct(ProductRequestDTO request)
        {
            var product = _mapper.Map<ProductEntity>(request);
            product.CreatedAt = DateTime.UtcNow;
            _unitOfWork.ProductRepository.Add(product);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<ProductResponseDTO>(product);
        }

        public async Task<ProductResponseDTO> UpdateProduct(ProductRequestDTO request)
        {
            if (request.Id == null) return null!;
            var existingProduct = await _unitOfWork.ProductRepository.GetByIdAsync(request.Id.Value, trackChanges: true);
            if (existingProduct == null) return null!;

            _mapper.Map(request, existingProduct);
            await _unitOfWork.ProductRepository.UpdateAsync(existingProduct);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<ProductResponseDTO>(existingProduct);
        }

        public async Task<bool> DeleteProduct(Guid id)
        {
            var existingProduct = await _unitOfWork.ProductRepository.GetByIdAsync(id, trackChanges: true);
            if (existingProduct == null) return false;

            _unitOfWork.ProductRepository.Remove(existingProduct);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<IEnumerable<ProductResponseDTO>> GetLowStockProducts()
        {
            var productEntities = await _unitOfWork.ProductRepository.GetQueryable()
                .Where(p => p.Quantity <= p.WarningThreshold)
                .ToListAsync();
            return _mapper.Map<List<ProductResponseDTO>>(productEntities);
        }

        // --- Danh mục ---
        public async Task<IEnumerable<CategoryResponseDTO>> GetAllCategories()
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllAsync();
            return _mapper.Map<List<CategoryResponseDTO>>(categories);
        }

        public async Task<CategoryResponseDTO> CreateCategory(CategoryRequestDTO request)
        {
            var category = _mapper.Map<CategoryEntity>(request);
            category.CreatedAt = DateTime.UtcNow;
            _unitOfWork.CategoryRepository.Add(category);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<CategoryResponseDTO>(category);
        }

        public async Task<CategoryResponseDTO> UpdateCategory(CategoryRequestDTO request)
        {
            if (request.Id == null) return null!;
            var existingCategory = await _unitOfWork.CategoryRepository.GetByIdAsync(request.Id.Value, trackChanges: true);
            if (existingCategory == null) return null!;

            _mapper.Map(request, existingCategory);
            await _unitOfWork.CategoryRepository.UpdateAsync(existingCategory);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<CategoryResponseDTO>(existingCategory);
        }

        public async Task<bool> DeleteCategory(Guid id)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id, trackChanges: true);
            if (category == null) return false;

            _unitOfWork.CategoryRepository.Remove(category);
            return await _unitOfWork.CompleteAsync() > 0;
        }
    }
}
