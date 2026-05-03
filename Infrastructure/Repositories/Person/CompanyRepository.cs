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
    internal class CompanyRepository : AccountRepository<Company>, ICompanyRepository
    {
        internal CompanyRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<Company?> GetByCVRAsync(string cvrNumber)
        {
            return await _context.Companies.FirstOrDefaultAsync(c => c.CVRNumber == cvrNumber);
        }
        public async Task<Company?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.Companies
                .Include(c => c.Employees)
                .Include(c => c.Projects)
                .Include(c => c.Activities)
                .Include(c => c.Expenses)
                .AsSplitQuery()
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
