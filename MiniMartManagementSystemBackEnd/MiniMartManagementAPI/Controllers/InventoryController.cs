using BackEnd.Core.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniMartManagementAPI.Service.Inventory;

namespace MiniMartManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        // --- Sản phẩm ---
        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts() => Ok(await _inventoryService.GetAllProducts());

        [HttpGet("GetProductsPaged")]
        public async Task<IActionResult> GetProductsPaged(int pageIndex = 1, int pageSize = 10) 
            => Ok(await _inventoryService.GetProductsPaged(pageIndex, pageSize));

        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductRequestDTO request) 
            => Ok(await _inventoryService.CreateProduct(request));

        [HttpPut("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductRequestDTO request)
        {
            var result = await _inventoryService.UpdateProduct(request);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("DeleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var result = await _inventoryService.DeleteProduct(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpGet("GetLowStockProducts")]
        public async Task<IActionResult> GetLowStockProducts() => Ok(await _inventoryService.GetLowStockProducts());

        // --- Danh mục ---
        [HttpGet("GetAllCategories")]
        public async Task<IActionResult> GetAllCategories() => Ok(await _inventoryService.GetAllCategories());

        [HttpPost("CreateCategory")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryRequestDTO request) 
            => Ok(await _inventoryService.CreateCategory(request));

        [HttpPut("UpdateCategory")]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryRequestDTO request)
        {
            var result = await _inventoryService.UpdateCategory(request);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("DeleteCategory/{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var result = await _inventoryService.DeleteCategory(id);
            if (!result) return NotFound();
            return Ok();
        }
    }
}
