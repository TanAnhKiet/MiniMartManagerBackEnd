using AutoMapper;
using BackEnd.Core.Domain.Entities;
using BackEnd.Core.Models;
using BackEnd.Core.Models.Request;
using BackEnd.Core.Models.Response;
using BackEnd.Core.Models.System;
using BackEnd.Core.SeedWorks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MiniMartManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("GetAllProduct")]
        public async Task<IActionResult> GetProduct()
        {
            var productEntities = await _unitOfWork.ProductRepository.GetAllAsync();
            var products = _mapper.Map<List<ProductResponseDTO>>(productEntities);
            return Ok(products);
        }

        [HttpGet("GetProductPaged")]
        public async Task<IActionResult> GetProductPaged(int pageIndex = 1, int pageSize = 10)
        {
            var pagedResult = await _unitOfWork.ProductRepository.GetPagedAsync(pageIndex, pageSize);
            var pagedDto = new PagedResult<ProductResponseDTO>
            {
                Items = _mapper.Map<List<ProductResponseDTO>>(pagedResult.Items),
                TotalCount = pagedResult.TotalCount,
                PageIndex = pagedResult.PageIndex,
                PageSize = pagedResult.PageSize
            };
            return Ok(pagedDto);
        }

        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductRequestDTO request)
        {
            if (request == null)
            {
                return BadRequest("Dữ liệu không hợp lệ");
            }

            var product = _mapper.Map<ProductEntity>(request);
            _unitOfWork.ProductRepository.Add(product);
            await _unitOfWork.CompleteAsync();

            var resultDto = _mapper.Map<ProductResponseDTO>(product);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, resultDto);
        }

        [HttpPut("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductRequestDTO request)
        {
            if (request == null || request.Id == Guid.Empty)
            {
                return BadRequest("Dữ liệu không hợp lệ");
            }

            var existingProduct = await _unitOfWork.ProductRepository.GetByIdAsync(request.Id.Value, trackChanges: true);
            if (existingProduct == null)
            {
                return NotFound("Sản phẩm không tồn tại");
            }

            _mapper.Map(request, existingProduct);
            await _unitOfWork.ProductRepository.UpdateAsync(existingProduct);
            await _unitOfWork.CompleteAsync();
            return Ok(_mapper.Map<ProductResponseDTO>(existingProduct));
        }

        [HttpDelete("DeleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Dữ liệu không hợp lệ");
            }

            var existingProduct = await _unitOfWork.ProductRepository.GetByIdAsync(id, trackChanges: true);
            if (existingProduct == null)
            {
                return NotFound("Sản phẩm không tồn tại");
            }

            _unitOfWork.ProductRepository.Remove(existingProduct);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
    }
}
