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
        // Bruges i ForgotPassword til at finde kontoen ud fra medarbejderens e-mail, legend!
        public async Task<Account?> GetByEmployeeEmailAsync(string email)
        {
            return await _context.Accounts
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.Employee != null && a.Employee.Email.Value == email.ToLower().Trim());
        }

        // Bruges i ResetPassword. Da RecoveryToken er privat, må vi hente alle konti med aktive tokens 
        // ind i hukommelsen (eller gøre feltet internal/public i domænet, hvis I hellere vil det)
        public async Task<Account?> GetByRecoveryTokenAsync(string token)
        {
            // Hvis I ikke vil hente det i hukommelsen, kan I ændre 'private string? RecorveryToken;' 
            // til 'public string? RecorveryToken { get; private set; }' i Account.cs!
            return await _context.Accounts
                .AsEnumerable()
                .FirstOrDefaultAsync(a => System.Reflection.ObfuscationAttribute.Equals(a.GetType().GetField("RecorveryToken", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(a), token));
        }


    }
}
