using Application.Interfaces.Repo.Item;
using Domain.Entity.Item;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories.Item
{
    internal class ProjectRepository : GenericRepository<Project>, IProjectRepository
    {
        internal ProjectRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Project>> GetByEmployeeIdAsync(Guid employeeId)
        {
            return await _context.Projects
                .Where(p => p.ResponsibleEmployeeId == employeeId)
                .ToListAsync();
        }
        public async Task<IEnumerable<Project>> GetByActivityIdAsync(Guid activityId)
        {
            return await _context.Projects
                .Where(p => p.Activities
                .Any(a => a.ActivityId == activityId))
                .ToListAsync();
        }
        public async Task<IEnumerable<Project>> GetByCompanyIdAsync(Guid companyId)
        {
            return await _context.Projects
                .Where(p => p.CompanyId == companyId)
                .ToListAsync();
        }
        public async Task<IEnumerable<Project>> GetProjectsRelatedToEmployeeAsync(Guid employeeId)
        {
            return await _context.Projects
                .Where(p => p.ResponsibleEmployeeId == employeeId ||
                            p.Registrations.Any(r => r.EmployeeId == employeeId) ||
                            p.Activities.Any(a => a.ResponsibleEmployeeId == employeeId) ||
                            p.Activities.Any(a => a.Registrations.Any(r => r.EmployeeId == employeeId)))
                .ToListAsync();
        }
        public async Task<Project?> GetByIdWithDetailsAsync(Guid projectId)
        {
            return await _context.Projects
                .Include(p => p.Activities)
                .Include(p => p.Registrations)
                .AsSplitQuery()
                .FirstOrDefaultAsync(p => p.Id == projectId);
        }
    }
}
