
using Domain.Entity.Mapping;
using Microsoft.EntityFrameworkCore;
using Domain.Entity.Mapping.ValueObjects;
using Domain.Interfaces.Repos;
using Domain.ValueObjects;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;
namespace Infrastructure.Repositories.Mappings
{
    internal class ProviderRepository : GenericRepository<Provider>, IProviderRepository
    {
        public ProviderRepository(AppDbContext context) : base(context)
        {
        }


        public async Task<Provider?> FindByDatasourceAsync(DataSource datasource)
        {
            return await _context.Providers
                .Include(p => p.Urls)
                .FirstOrDefaultAsync(p => p.Datasource == datasource);
        }

    }
}
