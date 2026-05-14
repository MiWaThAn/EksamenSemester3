using Domain.Entity.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Repo.Mapping
{
    public interface IIntegrationMappingRepository
    {
        Task<IEnumerable<IntegrationMapping>> GetByLocalId(Guid LocalId, IntegrationEntityType EntityType, CancellationToken cancellationToken = default);
        Task<IEnumerable<IntegrationMapping>> GetByExternalId(string ExternalId, IntegrationEntityType EntityType, CancellationToken cancellationToken = default);
    }
}
