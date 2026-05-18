using Domain.Entity.Mapping.ValueObjects;
using Domain.Guards;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity.Mapping
{
    public class ProviderEndpoint : Base
    {
        public Guid ProviderId { get; private set; }
        public IntegrationEntityType EntityType { get; private set; }
        public string Url { get; private set; }

        private ProviderEndpoint() { }

        internal ProviderEndpoint(IntegrationEntityType entityType, string url)
        {
            Guard.AgainstNull(entityType, nameof(entityType));
            Guard.AgainstNullOrEmpty(url, nameof(url));
            EntityType = entityType;
            Url = url;
        }

        internal void UpdateUrl(string url)
        {
            Guard.AgainstNullOrEmpty(url, nameof(url));
            Url = url;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}

