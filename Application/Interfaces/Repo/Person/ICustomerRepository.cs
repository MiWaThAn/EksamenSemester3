using Domain.Entity.Person;
using Domain.Interfaces.Repos;
using System;

using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Repo.Person
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        Task<Customer?> GetByCompanyIdAsync(Guid companyId);
    }
}
