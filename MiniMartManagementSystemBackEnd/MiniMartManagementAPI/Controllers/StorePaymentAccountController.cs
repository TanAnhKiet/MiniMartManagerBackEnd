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
    public class StorePaymentAccountController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StorePaymentAccountController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("GetStorePaymentAccountByStoreId/{storeId}")]
        [Authorize(Roles = "RootAdmin, SystemEngineer")]
        public async Task<IActionResult> GetStorePaymentAccountByStoreId(Guid storeId)
        {
            var storePaymentAccounts = await _unitOfWork.StorePaymentAccountRepository.GetAllByIdAsync(storeId);
            if (storePaymentAccounts == null)
            {
                return Ok(new List<StorePaymentAccountResponseDTO>());
            }
            var storePaymentAccountDtos = _mapper.Map<List<StorePaymentAccountResponseDTO>>(storePaymentAccounts);
            return Ok(storePaymentAccountDtos);
        }

        [HttpPost("CreateStorePaymentAccount")]
        [Authorize(Roles = "RootAdmin, SystemEngineer")]
        public async Task<IActionResult> CreateStorePaymentAccount([FromBody] StorePaymentAccountRequestDTO request)
        {
            var storePaymentAccount = _mapper.Map<StorePaymentAccountEntity>(request);
            storePaymentAccount.CreatedAt = DateTime.UtcNow;
            _unitOfWork.StorePaymentAccountRepository.Add(storePaymentAccount);
            await _unitOfWork.CompleteAsync();
            var storePaymentAccountDto = _mapper.Map<StorePaymentAccountResponseDTO>(storePaymentAccount);
            return Ok(storePaymentAccountDto);
        }

        [HttpPut("UpdateStorePaymentAccount")]
        [Authorize(Roles = "RootAdmin, SystemEngineer")]
        public async Task<IActionResult> UpdateStorePaymentAccount([FromBody] StorePaymentAccountRequestDTO request)
        {
            if (request == null || !request.Id.HasValue)
            {
                return BadRequest();
            }

            var existingAccount = await _unitOfWork.StorePaymentAccountRepository.GetByIdAsync(request.Id.Value, trackChanges: true);
            if (existingAccount == null)
            {
                return NotFound();
            }

            _mapper.Map(request, existingAccount);
            await _unitOfWork.StorePaymentAccountRepository.UpdateAsync(existingAccount);
            await _unitOfWork.CompleteAsync();
            var storePaymentAccountDto = _mapper.Map<StorePaymentAccountResponseDTO>(existingAccount);
            return Ok(storePaymentAccountDto);
        }

        [HttpDelete("DeleteStorePaymentAccount/{id}")]
        [Authorize(Roles = "RootAdmin, SystemEngineer")]
        public async Task<IActionResult> DeleteStorePaymentAccount(Guid id)
        {
            var storePaymentAccount = await _unitOfWork.StorePaymentAccountRepository.GetByIdAsync(id, trackChanges: true);
            if (storePaymentAccount == null)
            {
                return NotFound();
            }
            _unitOfWork.StorePaymentAccountRepository.Remove(storePaymentAccount);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }
    }
}