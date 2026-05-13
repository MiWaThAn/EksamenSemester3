using Domain.Entity.Mapping;
using Domain.Entity.Mapping.ValueObjects;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces.Mapping
{
    public interface IProviderFactory
    {
        Task<Result<Provider>> CreateAsync(DataSource datasource, Dictionary<IntegrationEntityType, string> urls);
    }
}
