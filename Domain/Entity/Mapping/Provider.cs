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
        private readonly Dictionary<IntegrationEntityType, string> _urls = new();  

        public IReadOnlyDictionary<IntegrationEntityType, string> Urls => _urls;
        //settings.Provider.Urls[IntegrationEntityType.Employee] = "https://restapi.e-conomic.com/employees";
        

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

            if (_urls.ContainsKey(entityType))
                throw new Exception($"URL for '{entityType}' already exists.");

            _urls[entityType] = url;
        }

        public void UpdateUrl(IntegrationEntityType entityType, string url)
        {
            Guard.AgainstNull(entityType, nameof(entityType));
            Guard.AgainstNullOrEmpty(url, nameof(url));

            _urls[entityType] = url;
        }

        public void RemoveUrl(IntegrationEntityType entityType)
        {
            Guard.AgainstNull(entityType, nameof(entityType));

            _urls.Remove(entityType);
        }


    }

        
}
    
