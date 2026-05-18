using Application.Adapters;
using Domain.Entity.Mapping.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Adapters
{
    public interface IProviderAdapter
    {
        bool Supports(DataSource datasource);
        IEnumerable<SyncEntity> Map(string json, IntegrationEntityType entityType, Guid companyId);
    }
}
