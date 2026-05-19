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
        public async Task<IEnumerable<Project>> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
        {
            return await _context.Projects
                .Where(p => p.ResponsibleEmployeeId == employeeId)
                .ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<Project>> GetByActivityIdAsync(Guid activityId, CancellationToken cancellationToken = default)
        {
            return await _context.Projects
                .Where(p => p.Activities
                .Any(a => a.ActivityId == activityId))
                .ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<Project>> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default)
        {
            return await _context.Projects
                .Where(p => p.CompanyId == companyId)
                .ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<Project>> GetProjectsRelatedToEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default)
        {
            return await _context.Projects
                .Where(p => p.ResponsibleEmployeeId == employeeId ||
                            p.WorkLogs.Any(r => r.EmployeeId == employeeId) ||
                            p.WorkLogs.Any(r => r.ActiveRegistrations.Any(r=>r.EmployeeId == employeeId)) ||
                            p.Activities.Any(a => a.ResponsibleEmployeeId == employeeId) ||
                            p.Activities.Any(a => a.Registrations.Any(r => r.EmployeeId == employeeId)))
                .ToListAsync(cancellationToken);
        }
        public async Task<Project?> GetByIdWithDetailsAsync(Guid projectId, CancellationToken cancellationToken = default)
        {
            return await _context.Projects
                .Include(p => p.Activities)
                .Include(p => p.WorkLogs)
                .AsSplitQuery()
                .FirstOrDefaultAsync(p => p.Id == projectId,cancellationToken);
        }
    }
}
