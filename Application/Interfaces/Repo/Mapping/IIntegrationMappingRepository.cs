using Domain.Entity.Mapping;
using Domain.Entity.Mapping.ValueObjects;
using Domain.Interfaces.Repos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Repo.Mapping
{
    public interface IIntegrationMappingRepository : IGenericRepository<IntegrationMapping>
    {
        Task<IEnumerable<IntegrationMapping>> GetByLocalId(Guid LocalId, IntegrationEntityType EntityType, CancellationToken cancellationToken = default);
        Task<IEnumerable<IntegrationMapping>> GetByExternalId(string ExternalId, IntegrationEntityType EntityType, CancellationToken cancellationToken = default);
    }
}
