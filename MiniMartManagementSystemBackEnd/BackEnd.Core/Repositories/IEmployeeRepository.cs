using BackEnd.Core.Domain.Entities;
using BackEnd.Core.SeedWorks;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackEnd.Core.Repositories
{
    public interface IEmployeeRepository : IRepository<EmployeeEntity, Guid>
    {
        Task<EmployeeEntity> UpdateAsync(EmployeeEntity employee);
        Task<int> CountAsync();
    }
}
