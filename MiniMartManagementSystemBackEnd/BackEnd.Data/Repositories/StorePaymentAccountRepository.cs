using AutoMapper;
using BackEnd.Core.Domain.Entities;
using BackEnd.Core.Repositories;
using BackEnd.Data.SeedWorks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackEnd.Data.Repositories
{
    public class StorePaymentAccountRepository : RepositoryBase<StorePaymentAccountEntity, Guid>, IStorePaymentAccountRepository
    {
        private readonly IMapper _mapper;
        public StorePaymentAccountRepository(DBContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public async Task<List<StorePaymentAccountEntity>> GetAllByIdAsync(Guid id)
        {
            return await _context.Set<StorePaymentAccountEntity>()
                         .AsNoTracking() // Tối ưu: Không theo dõi thay đổi nếu chỉ để đọc
                         .Where(x => x.StoreId == id)
                         .ToListAsync();
        }

        public async Task<StorePaymentAccountEntity> UpdateAsync(StorePaymentAccountEntity storePaymentAccount)
        {
            _context.Set<StorePaymentAccountEntity>().Update(storePaymentAccount);
            return await Task.FromResult(storePaymentAccount);
        }

        public async Task<StorePaymentAccountEntity?> GetByMerchantIdAsync(string merchantId)
        {
            return await _context.Set<StorePaymentAccountEntity>()
                         .AsNoTracking()
                         .FirstOrDefaultAsync(x => x.MerchantId == merchantId);
        }
    }
}
