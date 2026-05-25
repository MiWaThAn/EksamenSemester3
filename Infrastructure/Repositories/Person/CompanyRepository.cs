using Application.Interfaces.Repo.Person;
using Domain.Entity.Person;
using Domain.Interfaces.Repos;
using Domain.ValueObjects;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using Domain.Entity.Mapping;

namespace Infrastructure.Repositories.Person
{
    public class CompanyRepository : GenericRepository<Company>, ICompanyRepository
    {
        public CompanyRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<Company?> GetByCVRAsync(CvrNumber cvrNumber, CancellationToken cancellationToken = default)
        {
            return await _context.Companies.FirstOrDefaultAsync(c => c.CVRNumber == cvrNumber,cancellationToken);
        }
        public async Task<Company?> GetByCVRWithSettingsAsync(CvrNumber cvrNumber)
        {
            return await _context.Companies
                .Include(c => c.Settings)
                    .ThenInclude(s => s.Provider)
                        .ThenInclude(p => p.Urls)
                .Include(c => c.Settings)
                    .ThenInclude(s => s.EntityTypes)
                .FirstOrDefaultAsync(c => c.CVRNumber == cvrNumber && !c.IsDeleted);
        }
        public async Task<Company?> GetWithAllDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Companies
                .Include(c => c.Employees)
                .Include(c => c.Projects)
                .Include(c => c.Activities)
                .Include(c => c.Expenses)
                .AsSplitQuery()
                .FirstOrDefaultAsync(c => c.Id == id,cancellationToken);
        }
        public async Task<Company?> GetByEmailAsync(EmailAddress emailAddress, CancellationToken cancellationToken = default)
        {
            return await _context.Companies.FirstOrDefaultAsync(c => c.Email == emailAddress,cancellationToken);
        }

        public async Task<Company?> GetWithProjectsAsync(Guid Id, CancellationToken cancellationToken = default)
        {
            return await _context.Companies.Include(c=>c.Projects).FirstOrDefaultAsync(c=>c.Id == Id,cancellationToken);
        }
        public async Task<Company?> GetWithEmployeesAsync(Guid Id, CancellationToken cancellationToken = default)
        {
            return await _context.Companies.Include(c => c.Employees).FirstOrDefaultAsync(c => c.Id == Id,cancellationToken);
        }
        public async Task<Company?> GetWithActivitiesAsync(Guid Id, CancellationToken cancellationToken = default)
        {
            return await _context.Companies.Include(c => c.Activities).FirstOrDefaultAsync(c => c.Id == Id,cancellationToken);
        }
        public async Task<Company?> GetWithExpensesAsync(Guid Id, CancellationToken cancellationToken = default)
        {
            return await _context.Companies.Include(c => c.Expenses).FirstOrDefaultAsync(c => c.Id == Id,cancellationToken);
        }
        public async Task<IEnumerable<Company?>> GetAllWithIntegrationSettingsAsync()
        {
            return await _context.Companies
            .Include(c => c.Settings)
                .ThenInclude(s => s.Provider)
                    .ThenInclude(p => p.Urls)
            .Include(c => c.Settings)
                .ThenInclude(s => s.EntityTypes)
            .Where(c => !c.IsDeleted)
            .AsSplitQuery()
            .ToListAsync();
        }
        public async Task<Company?> GetByAccountIdAsync(Guid accountId)
        {
            return await _context.Companies
                .Include(c => c.Settings)
                    .ThenInclude(s => s.Provider)
                        .ThenInclude(p => p.Urls)
                .Include(c => c.Settings)
                    .ThenInclude(s => s.EntityTypes)
                .AsSplitQuery()
                .FirstOrDefaultAsync(c => c.AccountId == accountId);
        }
    }
}
