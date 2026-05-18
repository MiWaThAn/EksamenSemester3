using System;
using System.Collections.Generic;
using System.Text;
using Domain.Guards;
using Domain.Entity.Mapping.ValueObjects;
namespace Domain.Entity.Mapping
{
    public class Provider : Base
    {
        
        public DataSource Datasource { get; private set; }
        private readonly List<ProviderEndpoint> _urls = new();
        public IReadOnlyCollection<ProviderEndpoint> Urls => _urls.AsReadOnly();
        //settings.Provider.Urls[IntegrationEntityType.Employee] = "https://restapi.e-conomic.com/employees";

        public Provider() { }
        internal Provider(DataSource datasource, Dictionary<IntegrationEntityType, string> urls)
            {
            Guard.AgainstNull(datasource, nameof(datasource));
            Guard.AgainstNull(urls, nameof(urls));
            
            Datasource = datasource; 
            foreach (var (entityType, url) in urls)
            {
                AddUrl(entityType, url);  
            }
            }

        public void AddUrl(IntegrationEntityType entityType, string url)
        {
            Guard.AgainstNull(entityType, nameof(entityType));
            Guard.AgainstNullOrEmpty(url, nameof(url));

            if (_urls.Any(u => u.EntityType == entityType))
                throw new Exception($"URL for '{entityType}' already exists.");

            _urls.Add(new ProviderEndpoint(entityType, url));
        }

        public void UpdateUrl(IntegrationEntityType entityType, string url)
        {
            Guard.AgainstNull(entityType, nameof(entityType));
            Guard.AgainstNullOrEmpty(url, nameof(url));

            var existing = _urls.FirstOrDefault(u => u.EntityType == entityType);
            if (existing == null)
                throw new Exception($"No URL registered for '{entityType}'.");

            existing.UpdateUrl(url);
        }

        public void RemoveUrl(IntegrationEntityType entityType)
        {
            Guard.AgainstNull(entityType, nameof(entityType));

            var existing = _urls.FirstOrDefault(u => u.EntityType == entityType);
            if (existing == null)
                throw new Exception($"No URL registered for '{entityType}'.");

            _urls.Remove(existing);
        }
        public bool SupportsEntityType(IntegrationEntityType entityType)
        {
            return _urls.Any(u => u.EntityType == entityType);
        }

        public void ValidateEntityTypes(List<IntegrationEntityType> entityTypes)
        {
            var unsupported = entityTypes
                .Where(e => !SupportsEntityType(e))
                .ToList();

            if (unsupported.Any())
                throw new Exception(
                    $"Provider does not support: {string.Join(", ", unsupported)}");
        }

    }

        
}
    
