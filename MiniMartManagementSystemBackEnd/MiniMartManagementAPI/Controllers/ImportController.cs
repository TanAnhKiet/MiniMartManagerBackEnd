using BackEnd.Core.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniMartManagementAPI.Service.Import;

namespace MiniMartManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ImportController : ControllerBase
    {
        private readonly IImportService _importService;

        public ImportController(IImportService importService)
        {
            _importService = importService;
        }

        // --- Nhập hàng ---
        [HttpPost("CreateImport")]
        public async Task<IActionResult> CreateImport([FromBody] ImportRequestDTO request) 
            => Ok(await _importService.CreateImport(request));

        [HttpGet("GetAllImports")]
        public async Task<IActionResult> GetAllImports() => Ok(await _importService.GetAllImports());

        [HttpGet("GetImportById/{id}")]
        public async Task<IActionResult> GetImportById(Guid id)
        {
            var result = await _importService.GetImportById(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // --- Nhà cung cấp ---
        [HttpGet("GetAllSuppliers")]
        public async Task<IActionResult> GetAllSuppliers() => Ok(await _importService.GetAllSuppliers());

        [HttpPost("CreateSupplier")]
        public async Task<IActionResult> CreateSupplier([FromBody] SupplierRequestDTO request) 
            => Ok(await _importService.CreateSupplier(request));

        [HttpPut("UpdateSupplier")]
        public async Task<IActionResult> UpdateSupplier([FromBody] SupplierRequestDTO request)
        {
            var result = await _importService.UpdateSupplier(request);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("DeleteSupplier/{id}")]
        public async Task<IActionResult> DeleteSupplier(Guid id)
        {
            var result = await _importService.DeleteSupplier(id);
            if (!result) return NotFound();
            return Ok();
        }
    }
}
