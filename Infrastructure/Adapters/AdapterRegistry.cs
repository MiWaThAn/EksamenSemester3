using Application.Interfaces.Adapters;
using Domain.Entity.Mapping.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;
using Application.Interfaces.Registries;
namespace Infrastructure.Adapters
{
    public class AdapterRegistry : IAdapterRegistry
    {

        private readonly IEnumerable<IProviderAdapter> _adapters;

        public AdapterRegistry(IEnumerable<IProviderAdapter> adapters)
        {
            _adapters = adapters;
        }

        public IProviderAdapter GetAdapter(DataSource datasource)
        {
            var adapter = _adapters.FirstOrDefault(a => a.Supports(datasource));
            if (adapter == null)
                throw new Exception($"No adapter registered for '{datasource}'.");
            return adapter;
        }

    }
}
