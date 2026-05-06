using Application.Interfaces.Repo.Person;
using Domain.Entity.Person;
using Domain.Interfaces.Repos;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories.Person
{
    internal class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        public AccountRepository(AppDbContext context) : base(context) { }
        public async Task<Account?> GetByUsernameAsync(string username)
        {
            throw new NotImplementedException();
        }
        public async Task<Account?> GetByPhoneNumberAsync(string phoneNumber)
        {
            throw new NotImplementedException();
        }
        public async Task<Account?> GetWithCompanyAsync(string username)
        {
            throw new NotImplementedException();
        }
        public async Task<Account?> GetWithEmployeeAsync(string username)
        {
            throw new NotImplementedException();
        }
        public async Task<Account?> GetWithEmployeeAndCompany(string username)
        {
            throw new NotImplementedException();
        }
    }
}
