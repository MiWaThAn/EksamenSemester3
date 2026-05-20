using Application.Interfaces.Repo.Item;
using Domain.Entity.Item;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Domain.Entity.Item.Activities;
using Activity = Domain.Entity.Item.Activities.Activity;

namespace Infrastructure.Repositories.Item
{
    public class ActivityRepository : GenericRepository<Activity>, IActivityRepository
    {
        public ActivityRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Activity>> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default)
        {
            return await _context.Activities
                .Where(a => a.CompanyId == companyId)
                .ToListAsync(cancellationToken);
        }
    }
}
