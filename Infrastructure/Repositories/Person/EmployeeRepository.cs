using Application.Interfaces.Repo.Person;
using Domain.Entity.Person;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Model;
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
        public async Task<Employee?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.Set<Employee>()
                .Include(e => e.Registrations)
                .AsSplitQuery()
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<CompanyEmployeeModel>?> GetEmployeesRelatedToProjectAsync(Guid projectId)
        {
            return await _context.Employees
                .Where(e =>
                    _context.Projects.Any(p => p.Id == projectId && p.ResponsibleEmployeeId == e.Id) ||
                    e.Registrations.Any(r => r.ProjectId == projectId) ||
                    _context.Projects.Any(p => p.Id == projectId &&
                        p.Activities.Any(pa => pa.ResponsibleEmployeeId == e.Id)) ||
                    e.Registrations.Any(r =>
                        _context.Projects.Any(p => p.Id == projectId &&
                            p.Activities.Any(pa => pa.Id == r.ProjectActivityId))))
                .Select(e => new CompanyEmployeeModel
                {
                    Id = e.Id,
                    FullName = e.Name
                })
                .ToListAsync();
        }

    }
}
