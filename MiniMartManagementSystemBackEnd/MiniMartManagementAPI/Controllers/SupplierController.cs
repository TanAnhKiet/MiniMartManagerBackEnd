using AutoMapper;
using BackEnd.Core.Domain.Entities;
using BackEnd.Core.Models;
using BackEnd.Core.Models.Request;
using BackEnd.Core.Models.Response;
using BackEnd.Core.SeedWorks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MiniMartManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SupplierController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SupplierController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("GetAllSupplier")]
        public async Task<IActionResult> GetAllSupplier()
        {
            var suppliers = await _unitOfWork.SupplierRepository.GetAllAsync();
            var supplierDtos = _mapper.Map<List<SupplierResponseDTO>>(suppliers);
            return Ok(supplierDtos);
        }

        [HttpPost("CreateSupplier")]
        public async Task<IActionResult> CreateSupplier([FromBody] SupplierRequestDTO request)
        {
            var supplier = _mapper.Map<SupplierEntity>(request);
            supplier.Phone ??= "";
            supplier.Address ??= "";
            supplier.CreatedAt = DateTime.UtcNow;
            _unitOfWork.SupplierRepository.Add(supplier);
            await _unitOfWork.CompleteAsync();
            var supplierDto = _mapper.Map<SupplierResponseDTO>(supplier);
            return Ok(supplierDto);
        }

        [HttpPut("UpdateSupplier")]
        public async Task<IActionResult> UpdateSupplier([FromBody] SupplierRequestDTO request)
        {
            if (request == null || !request.Id.HasValue)
            {
                return BadRequest();
            }

            var existingSupplier = await _unitOfWork.SupplierRepository.GetByIdAsync(request.Id.Value, trackChanges: true);
            if (existingSupplier == null)
            {
                return NotFound();
            }

            _mapper.Map(request, existingSupplier);
            await _unitOfWork.SupplierRepository.UpdateAsync(existingSupplier);
            await _unitOfWork.CompleteAsync();
            var supplierDto = _mapper.Map<SupplierResponseDTO>(existingSupplier);
            return Ok(supplierDto);
        }

        [HttpDelete("DeleteSupplier/{id}")]
        public async Task<IActionResult> DeleteSupplier(Guid id)
        {
            var supplier = await _unitOfWork.SupplierRepository.GetByIdAsync(id, trackChanges: true);
            if (supplier == null)
            {
                return NotFound();
            }
            _unitOfWork.SupplierRepository.Remove(supplier);
            await _unitOfWork.CompleteAsync();
            return Ok();
        }
    }
}

