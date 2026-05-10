using AutoMapper;
using BackEnd.Core.Domain.Entities;
using BackEnd.Core.Models;
using BackEnd.Core.SeedWorks;
using Microsoft.EntityFrameworkCore;
using MiniMartManagementAPI.Service.Promotions;

namespace MiniMartManagementAPI.Service
{
    public class PromotionService : IPromotionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PromotionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PromotionDTO>> GetAllPromotions(Guid storeId)
        {
            var promotions = await _unitOfWork.PromotionRepository.GetQueryable()
                .Include(p => p.Product)
                .Where(p => p.StoreId == storeId)
                .ToListAsync();
            return _mapper.Map<IEnumerable<PromotionDTO>>(promotions);
        }

        public async Task<PromotionDTO> GetPromotionById(Guid id)
        {
            var promotion = await _unitOfWork.PromotionRepository.GetQueryable()
                .Include(p => p.Product)
                .FirstOrDefaultAsync(p => p.Id == id);
            return _mapper.Map<PromotionDTO>(promotion);
        }

        public async Task<PromotionDTO> CreatePromotion(PromotionRequestDTO request)
        {
            var promotion = _mapper.Map<PromotionEntity>(request);
            promotion.Id = Guid.NewGuid();
            promotion.CreatedAt = DateTime.UtcNow;
            
            _unitOfWork.PromotionRepository.Add(promotion);
            await _unitOfWork.CompleteAsync();
            
            return _mapper.Map<PromotionDTO>(promotion);
        }

        public async Task<PromotionDTO> UpdatePromotion(Guid id, PromotionRequestDTO request)
        {
            var promotion = await _unitOfWork.PromotionRepository.GetByIdAsync(id);
            if (promotion == null) return null!;

            _mapper.Map(request, promotion);
            await _unitOfWork.PromotionRepository.UpdateAsync(promotion);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<PromotionDTO>(promotion);
        }

        public async Task<bool> DeletePromotion(Guid id)
        {
            var promotion = await _unitOfWork.PromotionRepository.GetByIdAsync(id);
            if (promotion == null) return false;

            _unitOfWork.PromotionRepository.Remove(promotion);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<decimal> CalculateDiscount(Guid storeId, PromotionScope scope, Guid? productId, decimal originalPrice, int quantity)
        {
            var activePromotions = await _unitOfWork.PromotionRepository.GetActivePromotions(storeId, scope);
            
            // Lọc các KM áp dụng cho sản phẩm này hoặc cho toàn bộ cửa hàng (nếu ProductId null trong entity)
            // Trong thiết kế hiện tại, KM thường gắn với 1 sản phẩm. 
            // Nếu muốn KM cho toàn hóa đơn thì cần thiết kế thêm, nhưng theo file text là FK -> SanPham.
            
            var applicablePromotions = activePromotions.Where(p => p.ProductId == productId);

            decimal totalDiscount = 0;

            foreach (var promotion in applicablePromotions)
            {
                var strategy = PromotionStrategyFactory.GetStrategy(promotion.Type);
                totalDiscount += strategy.CalculateDiscount(promotion, originalPrice, quantity);
            }

            return totalDiscount;
        }
    }
}
