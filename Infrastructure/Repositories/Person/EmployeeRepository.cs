using Application.Interfaces.Repo.Person;
using Domain.Entity.Person;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories.Person
{
    internal class EmployeeRepository : AccountRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<Employee?> GetByEmployeeNumberAsync(string employeeNumber)
        {
            return await _context.Set<Employee>().FirstOrDefaultAsync(e => e.ExternalId == employeeNumber);
        }
        public async Task<Employee?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.Set<Employee>()
                .Include(e => e.Registrations)
                .AsSplitQuery()
                .FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}
