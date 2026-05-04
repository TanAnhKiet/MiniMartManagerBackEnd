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
    public class CategoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("GetAllCategory")]
        public async Task<ActionResult> GetAllCategory()
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllAsync();
            var categoryDtos = _mapper.Map<List<CategoryResponseDTO>>(categories);
            return Ok(categoryDtos);
        }

        [HttpPost("CreateCategory")]
        public async Task<ActionResult> CreateCategory([FromBody] CategoryRequestDTO request)
        {
            if (request == null) {
                return BadRequest();
            }
            var category = _mapper.Map<CategoryEntity>(request);
            category.CreatedAt = DateTime.UtcNow;
            _unitOfWork.CategoryRepository.Add(category);
            await _unitOfWork.CompleteAsync();
            return Ok(_mapper.Map<CategoryResponseDTO>(category));
        }

        [HttpPut("UpdateCategory")]
        public async Task<ActionResult> UpdateCategory([FromBody] CategoryRequestDTO request)
        {
            if (request == null || !request.Id.HasValue)
            {
                return BadRequest();
            }
            
            var existingCategory = await _unitOfWork.CategoryRepository.GetByIdAsync(request.Id.Value, trackChanges: true);
            if (existingCategory == null)
            {
                return NotFound();
            }

            _mapper.Map(request, existingCategory);
            await _unitOfWork.CategoryRepository.UpdateAsync(existingCategory);
            await _unitOfWork.CompleteAsync();
            return Ok(_mapper.Map<CategoryResponseDTO>(existingCategory));
        }

        [HttpDelete("DeleteCategory/{id}")]
        public async Task<ActionResult> DeleteCategory(Guid id)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id, trackChanges: true);
            if (category == null)
            {
                return NotFound();
            }
            _unitOfWork.CategoryRepository.Remove(category);
            await _unitOfWork.CompleteAsync();
            return Ok();
        }
    }
}

