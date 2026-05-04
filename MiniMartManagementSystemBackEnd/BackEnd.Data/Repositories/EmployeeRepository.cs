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
    public class EmployeeRepository : RepositoryBase<EmployeeEntity, Guid>, IEmployeeRepository
    {
        private readonly IMapper _mapper;
        public EmployeeRepository(DBContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public async Task<int> CountAsync()
        {
            return await _context.Set<EmployeeEntity>().AsNoTracking().CountAsync();
        }

        public async Task<EmployeeEntity> UpdateAsync(EmployeeEntity employee)
        {
            _context.Set<EmployeeEntity>().Update(employee);
            return await Task.FromResult(employee);
        }
    }
}
