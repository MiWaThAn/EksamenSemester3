using Domain.Entity.Person;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces.Repos
{
    public interface IAccountRepository : IGenericRepository<Account>
    {
        Task<Account?> GetByUsernameAsync(string username);
        Task<Account?> GetByPhoneNumberAsync(string phoneNumber);
        Task<Account?> GetWithCompanyAsync(string username);
        Task<Account?> GetWithEmployeeAsync(string username);
        Task<Account?> GetWithEmployeeAndCompany(string username);
    }
}