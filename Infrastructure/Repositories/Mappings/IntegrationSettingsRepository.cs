using Application.Interfaces.Repo.Mapping;
using Domain.Entity.Mapping;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories.Mappings
{
    internal class IntegrationSettingsRepository : GenericRepository<IntegrationSetting>, IIntegrationSettingsRepository
    {
        public IntegrationSettingsRepository(AppDbContext context):base(context)
        { }

        public async Task<IEnumerable<IntegrationSetting>> GetByCompanyId(Guid CompanyId, CancellationToken cancellationToken = default)
        {
            return await _context.IntegrationSettings.Where(i => i.CompanyId == CompanyId).ToListAsync(cancellationToken);
        }
    }
}
