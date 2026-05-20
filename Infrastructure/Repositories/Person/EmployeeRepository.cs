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
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<Employee?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Employee>()
                .Include(e => e.WorkLogs)
                .AsSplitQuery()
                .FirstOrDefaultAsync(e => e.Id == id,cancellationToken);
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

        public async Task<Employee?> GetByIdWithAccountAsync(Guid employeeId)
        {
            return await _context.Employees
                .Include(e => e.Account)
                .FirstOrDefaultAsync(e => e.Id == employeeId);
        }

    }
}
