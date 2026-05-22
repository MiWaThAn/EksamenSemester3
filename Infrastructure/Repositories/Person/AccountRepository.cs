using Application.Interfaces.Repo.Person;
using Domain.Entity.Person;
using Domain.Interfaces.Repos;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity.Mapping;
namespace Infrastructure.Repositories.Person
{
    internal class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        public AccountRepository(AppDbContext context) : base(context) { }
        public async Task<Account?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            return await _context.Accounts.FirstOrDefaultAsync(a => a.Username == username,cancellationToken);
        }
        public async Task<Account?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
        {
            return await _context.Accounts.FirstOrDefaultAsync(a => a.PhoneNumber.Value == phoneNumber, cancellationToken);
        }
        public async Task<Account?> GetWithCompanyAsync(string username, CancellationToken cancellationToken = default)
        {
            return await _context.Accounts.Include(a=>a.Company).FirstOrDefaultAsync(a => a.Username == username, cancellationToken);
        }
        public async Task<Account?> GetWithEmployeeAsync(string username, CancellationToken cancellationToken = default)
        {
            return await _context.Accounts.Include(a => a.Employee).FirstOrDefaultAsync(a => a.Username == username, cancellationToken);
        }
        public async Task<Account?> GetWithEmployeeAndCompany(string username, CancellationToken cancellationToken = default)
        {
            return await _context.Accounts.Include(a => a.Employee).Include(a => a.Company).FirstOrDefaultAsync(a => a.Username == username, cancellationToken);
        }
        public async Task<Account?> GetByEmployeeEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var formattedEmail = email.ToLower().Trim();

            return _context.Accounts
                .Include(a => a.Employee)
                .AsEnumerable()
                .FirstOrDefault(a => a.Employee != null &&
                                     a.Employee.Email != null &&
                                     a.Employee.Email.Value == formattedEmail);
        }

    }
}
