using BackEnd.Core.Models;
using BackEnd.Core.Models.Request;
using BackEnd.Core.Models.Response;
using BackEnd.Core.Models.System;

namespace MiniMartManagementAPI.Service.Inventory
{
    public interface IInventoryService
    {
        // Sản phẩm
        Task<IEnumerable<ProductResponseDTO>> GetAllProducts();
        Task<PagedResult<ProductResponseDTO>> GetProductsPaged(int pageIndex, int pageSize);
        Task<ProductResponseDTO> CreateProduct(ProductRequestDTO request);
        Task<ProductResponseDTO> UpdateProduct(ProductRequestDTO request);
        Task<bool> DeleteProduct(Guid id);
        Task<IEnumerable<ProductResponseDTO>> GetLowStockProducts();

        // Danh mục
        Task<IEnumerable<CategoryResponseDTO>> GetAllCategories();
        Task<CategoryResponseDTO> CreateCategory(CategoryRequestDTO request);
        Task<CategoryResponseDTO> UpdateCategory(CategoryRequestDTO request);
        Task<bool> DeleteCategory(Guid id);
    }
}
