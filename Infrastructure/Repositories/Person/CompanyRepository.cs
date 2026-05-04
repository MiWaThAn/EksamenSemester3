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

namespace Infrastructure.Repositories.Person
{
    internal class CompanyRepository : GenericRepository<Company>, ICompanyRepository
    {
        internal CompanyRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<Company?> GetByCVRAsync(CvrNumber cvrNumber)
        {
            return await _context.Companies.FirstOrDefaultAsync(c => c.CVRNumber == cvrNumber);
        }
        public async Task<Company?> GetWithAllDetailsAsync(Guid id)
        {
            return await _context.Companies
                .Include(c => c.Employees)
                .Include(c => c.Projects)
                .Include(c => c.Activities)
                .Include(c => c.Expenses)
                .AsSplitQuery()
                .FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<Company?> GetByEmailAsync(EmailAddress emailAddress)
        {
            return await _context.Companies.FirstOrDefaultAsync(c => c.Email == emailAddress);
        }

        public async Task<Company?> GetWithProjectsAsync(Guid Id)
        {
            return await _context.Companies.Include(c=>c.Projects).FirstOrDefaultAsync(c=>c.Id == Id);
        }
        public async Task<Company?> GetWithEmployeesAsync(Guid Id)
        {
            return await _context.Companies.Include(c => c.Employees).FirstOrDefaultAsync(c => c.Id == Id);
        }
        public async Task<Company?> GetWithActivitiesAsync(Guid Id)
        {
            return await _context.Companies.Include(c => c.Activities).FirstOrDefaultAsync(c => c.Id == Id);
        }
        public async Task<Company?> GetWithExpensesAsync(Guid Id)
        {
            return await _context.Companies.Include(c => c.Expenses).FirstOrDefaultAsync(c => c.Id == Id);
        }
    }
}
