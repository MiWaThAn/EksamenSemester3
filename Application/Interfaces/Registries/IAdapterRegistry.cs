using Application.Interfaces.Adapters;
using Domain.Entity.Mapping.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Registries
{
    public interface IAdapterRegistry
    {
        public IProviderAdapter GetAdapter(DataSource datasource);

    }
}
