using Application.Interfaces.Repo.Person;
using Domain.Entity.Person;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories.Person
{
    internal class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<Employee?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Employee>()
                .Include(e => e.Registrations)
                .AsSplitQuery()
                .FirstOrDefaultAsync(e => e.Id == id,cancellationToken);
        }
    }
}
