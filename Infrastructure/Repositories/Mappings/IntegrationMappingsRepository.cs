using Application.Interfaces.Repo.Mapping;
using Domain.Entity.Mapping;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories.Mappings
{
    internal class IntegrationMappingsRepository : GenericRepository<IntegrationMapping>, IIntegrationMappingRepository
    {
        //Create a dictionary somewhere.
        public IntegrationMappingsRepository(AppDbContext context): base(context) { }
        public async Task<IEnumerable<IntegrationMapping>> GetByLocalId(Guid LocalId, IntegrationEntityType EntityType)
        {
            return await _context.Mappings.Where(m => m.EntityType == EntityType && m.LocalId == LocalId).ToListAsync();
        }
        public async Task<IEnumerable<IntegrationMapping>> GetByExternalId(string ExternalId, IntegrationEntityType EntityType)
        {
            return await _context.Mappings.Where(m => m.EntityType == EntityType && m.ExternalId == ExternalId).ToListAsync();
        }
    }
}
