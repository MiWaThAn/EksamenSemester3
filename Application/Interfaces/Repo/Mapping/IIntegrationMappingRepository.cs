using Domain.Entity.Mapping;
using Domain.Entity.Mapping.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Repo.Mapping
{
    public interface IIntegrationMappingRepository
    {
        Task<IEnumerable<IntegrationMapping>> GetByLocalId(Guid LocalId, IntegrationEntityType EntityType);
        Task<IEnumerable<IntegrationMapping>> GetByExternalId(string ExternalId, IntegrationEntityType EntityType);
    }
}
