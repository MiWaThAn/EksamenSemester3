using Domain.Entity.Person;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces.Repos
{
    public interface IAccountRepository : IGenericRepository<Account>
    {
        Task<Account?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task<Account?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);
        Task<Account?> GetWithCompanyAsync(string username, CancellationToken cancellationToken = default);
        Task<Account?> GetWithEmployeeAsync(string username, CancellationToken cancellationToken = default);
        Task<Account?> GetWithEmployeeAndCompany(string username, CancellationToken cancellationToken = default);
        Task<Account?> GetByEmployeeEmailAsync(string email, CancellationToken cancellationToken = default);
    }
}