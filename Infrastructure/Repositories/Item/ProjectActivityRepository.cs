using Application.Interfaces.Repo.Item;
using Domain.Entity.Item.Activities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories.Item
{
    public class ProjectActivityRepository : GenericRepository<ProjectActivity>, IProjectActivityRepository
    {
        public ProjectActivityRepository(AppDbContext context) : base(context) { }
        public async Task<IEnumerable<ProjectActivity>> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
        {
            return await _context.ProjectActivities.Where(a => a.ResponsibleEmployeeId == employeeId).ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<ProjectActivity>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
        {
            return await _context.ProjectActivities.Where(pa=>pa.ProjectId == projectId).ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<ProjectActivity>> GetByIdsAsNoTrackingAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        {
            return await _context.ProjectActivities.AsNoTracking().Include(pa=>pa.Activity).Where(pa=>ids.Contains(pa.Id)).ToListAsync(cancellationToken);
        }
        public async Task<ProjectActivity?> GetByIdAsNoTrackingAsync(Guid Id, CancellationToken cancellationToken = default)
        {
            return await _context.ProjectActivities.AsNoTracking().Include(pa => pa.Activity).FirstOrDefaultAsync(pa=>pa.Id ==Id);
        }
    }
}
