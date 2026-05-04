using AutoMapper;
using BackEnd.Core.Domain.Entities;
using BackEnd.Core.Models.Request;
using BackEnd.Core.Models.Response;
using BackEnd.Core.Models.System;
using BackEnd.Core.SeedWorks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MiniMartManagementAPI.SinaglR;

namespace MiniMartManagementAPI.Controllers.Import
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "RootAdmin, Manager, Staff")]
    public class ImportController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHubContext<InventoryHub> _inventoryHub;    

        public ImportController(IUnitOfWork unitOfWork, IMapper mapper, IHubContext<InventoryHub> inventoryHub)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _inventoryHub = inventoryHub;   
        }

        [HttpGet("GetAllImportsPaged")]
        public async Task<IActionResult> GetAllImportsPaged(int pageIndex = 1, int pageSize = 10)
        {
            var pagedResult = await _unitOfWork.ImportRepository.GetPagedAsync(pageIndex, pageSize);
            var pagedDto = new PagedResult<ImportResponseDTO>
            {
                Items = _mapper.Map<List<ImportResponseDTO>>(pagedResult.Items),
                TotalCount = pagedResult.TotalCount,
                PageIndex = pagedResult.PageIndex,
                PageSize = pagedResult.PageSize
            };
            return Ok(pagedDto);
        }

        [HttpGet("GetAllImports")]
        public async Task<IActionResult> GetAllImports()
        {
            var imports = await _unitOfWork.ImportRepository.GetAllAsync();
            var importDtos = _mapper.Map<List<ImportResponseDTO>>(imports);
            return Ok(importDtos);
        }

        [HttpGet("GetImportById/{id}")]
        public async Task<IActionResult> GetImportById(Guid id)
        {
            var import = await _unitOfWork.ImportRepository.GetQueryable()
                .AsNoTracking()
                .Include(x => x.Items)
                    .ThenInclude(xi => xi.Product)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (import == null)
            {
                return NotFound("Không tìm thấy hóa đơn nhập hàng.");
            }
            var importDto = _mapper.Map<ImportResponseDTO>(import);
            return Ok(importDto);
        }

        [HttpPost("CreateImport")]
        public async Task<IActionResult> CreateImport([FromBody] ImportRequestDTO request)
        {
            if (request == null || !request.Items.Any())
            {
                return BadRequest("Dữ liệu hóa đơn không hợp lệ.");
            }

            var import = _mapper.Map<ImportEntity>(request);
            import.Id = Guid.NewGuid();
            import.CreatedAt = DateTime.Now;
            import.ImportCode = request.ImportCode ?? $"IMP-{DateTime.Now:yyyyMMddHHmmss}";
            import.Status = request.Status ?? "Success";
            import.TotalAmount = request.Items.Sum(x => x.Quantity * x.CostPrice);

            foreach (var item in import.Items)
            {
                item.Id = Guid.NewGuid();
                item.ImportId = import.Id;

                var product = await _unitOfWork.ProductRepository.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    product.Quantity += item.Quantity;
                    await _unitOfWork.ProductRepository.UpdateAsync(product);
                }
            }

             _unitOfWork.ImportRepository.Add(import);
            var result = await _unitOfWork.CompleteAsync();

            if (result > 0)
            {
                await _inventoryHub.Clients.All.SendAsync("UpdateInventory", "Kho đã cập nhật");
                var response = _mapper.Map<ImportResponseDTO>(import);
                return Ok(response);
            }

            return StatusCode(500, "Có lỗi xảy ra trong quá trình lưu dữ liệu.");
        }

        [HttpGet("GetImportByCode/{code}")]
        public IActionResult GetImportByCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return BadRequest("Mã phiếu nhập không được để trống");

            var imports = _unitOfWork.ImportRepository.Find(i => i.ImportCode.Contains(code));
            var importDtos = _mapper.Map<IEnumerable<ImportResponseDTO>>(imports);
            return Ok(importDtos);
        }

        [HttpGet("GetImportsByDate")]
        public IActionResult GetImportsByDate([FromQuery] DateTime date)
        {
            var imports = _unitOfWork.ImportRepository.Find(i => i.CreatedAt.Date == date.Date);
            var importDtos = _mapper.Map<IEnumerable<ImportResponseDTO>>(imports);
            return Ok(importDtos);
        }

        [HttpPut("CancelImport/{id}")]
        [Authorize(Roles = "RootAdmin, Manager")]
        public async Task<IActionResult> CancelImport(Guid id)
        {
            var import = await _unitOfWork.ImportRepository.GetQueryable()
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (import == null)
            {
                return NotFound("Không tìm thấy hóa đơn nhập hàng.");
            }

            if (import.Status == "Đã hủy" || import.Status == "Cancelled")
            {
                return BadRequest("Hóa đơn này đã được hủy trước đó.");
            }

            import.Status = "Đã hủy";

            foreach (var item in import.Items)
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    product.Quantity -= item.Quantity;
                    if (product.Quantity < 0) product.Quantity = 0;
                    await _unitOfWork.ProductRepository.UpdateAsync(product);
                }
            }

            var result = await _unitOfWork.CompleteAsync();

            if (result > 0)
            {
                await _inventoryHub.Clients.All.SendAsync("UpdateInventory", "Kho đã cập nhật");
                return Ok(new { message = "Hủy hóa đơn nhập hàng thành công." });
            }

            return StatusCode(500, "Có lỗi xảy ra trong quá trình cập nhật dữ liệu.");
        }
    }
}

