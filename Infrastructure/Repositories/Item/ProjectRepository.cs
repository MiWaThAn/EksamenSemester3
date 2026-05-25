using Application.Interfaces.Repo.Item;
using Domain.Entity.Item;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories.Item
{
    public class ProjectRepository : GenericRepository<Project>, IProjectRepository
    {
        public ProjectRepository(AppDbContext context) : base(context)
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
                .Where(p=>p.Assignments.Any(a=>a.EmployeeId == employeeId))
                .ToListAsync(cancellationToken);
        }
        public async Task<Project?> GetByIdWithDetailsAsync(Guid projectId, CancellationToken cancellationToken = default)
        {
            var query = _context.Projects
                .Include(p => p.Activities)
                    .ThenInclude(pa => pa.Activity)
                .Include(p => p.Registrations)
                .AsQueryable();

            // bruges til at gøre det muligt for in memory test at kunne håndtere include, da in memory ikke understøtter include på samme måde som en relationel database
            if (_context.Database.IsRelational())
            {
                query = query.AsSplitQuery();
            }

            return await query.FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken);
        }
    }
}
