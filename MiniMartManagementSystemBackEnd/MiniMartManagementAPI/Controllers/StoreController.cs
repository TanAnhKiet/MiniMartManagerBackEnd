using AutoMapper;
using BackEnd.Core.Domain.Entities;
using BackEnd.Core.Models;
using BackEnd.Core.Models.Request;
using BackEnd.Core.Models.Response;
using BackEnd.Core.SeedWorks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MiniMartManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "RootAdmin, Manager")]
    public class StoreController : ControllerBase
    {
       private readonly IUnitOfWork _unitOfWork;
       private readonly IMapper _mapper;

        public StoreController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("GetStoreById/{id}")]
        public async Task<IActionResult> GetStoreById(Guid id)
        {
            var store = await _unitOfWork.StoreRepository.GetByIdAsync(id);
            if (store == null)
            {
                return NotFound();
            }
            var storeDto = _mapper.Map<StoreResponseDTO>(store);
            return Ok(storeDto);
        }

        [HttpPut("UpdateStore")]
        [Authorize(Roles = "RootAdmin, Manager")]
        public async Task<IActionResult> UpdateStore([FromBody] StoreRequestDTO request)
        {
            if (request == null || !request.Id.HasValue)
            {
                return BadRequest();
            }
            
            var existingStore = await _unitOfWork.StoreRepository.GetByIdAsync(request.Id.Value, trackChanges: true);
            if (existingStore == null)
            {
                return NotFound();
            }

            _mapper.Map(request, existingStore);
            await _unitOfWork.StoreRepository.UpdateAsync(existingStore);
            await _unitOfWork.CompleteAsync();
            var storeDto = _mapper.Map<StoreResponseDTO>(existingStore);
            return Ok(storeDto);
        }
    }
}

