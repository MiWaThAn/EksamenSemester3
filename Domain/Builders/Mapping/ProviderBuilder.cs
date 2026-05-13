using Domain.Entity.Mapping;
using Domain.Entity.Mapping.ValueObjects;
using Domain.Guards;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Builders.Mapping
{
    internal class ProviderBuilder
    {

        public DataSource Datasource { get; private set; }
        public Dictionary<IntegrationEntityType, string> Urls { get; private set; } = new Dictionary<IntegrationEntityType, string>();





        public ProviderBuilder WithDataSource(DataSource datasource)
        {
            Datasource = datasource;
            return this;
        }
        public ProviderBuilder WithUrl(IntegrationEntityType entityType, string url)
        {
            Guard.AgainstNullOrEmpty(url, nameof(url));
            Urls[entityType] = url;
            return this;
        }
        public ProviderBuilder WithUrls(Dictionary<IntegrationEntityType, string> urls)
        {
            Guard.AgainstNull(urls, nameof(urls));
            foreach (var url in urls)
            {
                WithUrl(url.Key, url.Value); 
            }
            return this;
        }
        internal Provider Build()
        {
            if (Urls == null)
            {
                throw new InvalidOperationException("Urls cannot be null.");
            }
            if (Datasource == null)
            { 
                throw new InvalidOperationException("Datasource cannot be null.");
            }
                return new Provider(Datasource, Urls);

        }

    }
}
