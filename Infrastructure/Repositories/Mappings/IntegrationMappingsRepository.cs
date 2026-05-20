using Application.Interfaces.Repo.Mapping;
using Domain.Entity.Mapping;
using Domain.Entity.Mapping.ValueObjects;
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
        public async Task<IEnumerable<IntegrationMapping>> GetByLocalId(Guid LocalId, IntegrationEntityType EntityType, CancellationToken cancellationToken = default)
        {
            return await _context.Mappings.Where(m => m.EntityType == EntityType && m.LocalId == LocalId).ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<IntegrationMapping>> GetByExternalId(string ExternalId, IntegrationEntityType EntityType, CancellationToken cancellationToken = default)
        {
            return await _context.Mappings.Where(m => m.EntityType == EntityType && m.ExternalId == ExternalId).ToListAsync(cancellationToken);
        }
    }
}
