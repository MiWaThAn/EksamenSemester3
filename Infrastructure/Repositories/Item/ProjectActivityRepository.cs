using Application.Interfaces.Repo.Item;
using Domain.Entity.Item.Activities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories.Item
{
    internal class ProjectActivityRepository : GenericRepository<ProjectActivity>, IProjectActivityRepository
    {
        internal ProjectActivityRepository(AppDbContext context) : base(context) { }
        public async Task<IEnumerable<ProjectActivity>> GetByEmployeeIdAsync(Guid employeeId)
        {
            return await _context.ProjectActivities.Where(a => a.ResponsibleEmployeeId == employeeId).ToListAsync();
        }
        public async Task<IEnumerable<ProjectActivity>> GetByProjectIdAsync(Guid projectId)
        {
            return await _context.ProjectActivities.Where(pa=>pa.ProjectId == projectId).ToListAsync();
        }
    }
}
