using Domain.Entity.Person;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Repo.Person
{
    public interface ICustomerRepository : IAccountRepository<Customer>
    {
        Task<Customer?> GetByCustomerNumberAsync(string customerNumber);
        Task<Customer?> GetByCompanyIdAsync(Guid companyId);
    }
}
