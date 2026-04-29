using Application.Interfaces.Repo.Item;
using Domain.Entity.Item;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Domain.Entity.Item.Activity;
using Activity = Domain.Entity.Item.Activity.Activity;

namespace Infrastructure.Repositories.Item
{
    internal class ActivityRepository : GenericRepository<Activity>, IActivityRepository
    {
        internal ActivityRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Activity>> GetByCompanyIdAsync(Guid companyId)
        {
            return await _context.Activities
                .Where(a => a.CompanyId == companyId)
                .ToListAsync();
        }
        public async Task<Activity?> GetByActivityNumberAsync(string activityNumber)
        {
            return await _context.Activities.FirstOrDefaultAsync(a => a.ActivityNumber == activityNumber);
        }
    }
}
