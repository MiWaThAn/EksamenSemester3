using Domain.Builders.Mapping;
using Domain.Entity.Item.Registrations;
using Domain.Guards;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Domain.Entity.Mapping.ValueObjects;
namespace Domain.Entity.Mapping
{
    public class IntegrationSetting : Base
    {
        public Guid CompanyId { get; private set; }
        public Provider Provider { get; private set; }
        public Guid ProviderId { get; private set; } // F.eks. "Economic" eller "Dinero"
        private readonly List<SelectedEntityType> _entityTypes = new();
        public IReadOnlyCollection<SelectedEntityType> EntityTypes => _entityTypes.AsReadOnly();
        public IntegrationCredential? Credential { get; private set; }
        private readonly List<IntegrationMapping> _mappings = new();
        public IReadOnlyCollection<IntegrationMapping> Mappings => _mappings.Where(r => !r.IsDeleted).ToList().AsReadOnly();

        public IntegrationSetting() : base()
        {

        }
        internal IntegrationSetting(Guid companyId, Guid providerId, List<IntegrationEntityType> selectedEntityTypes,
    Provider provider, string key, string encryptedValue) : base()
        {
            Guard.AgainstEmptyGuid(companyId, nameof(companyId));
            Guard.AgainstEmptyGuid(providerId, nameof(providerId));
            Guard.AgainstNullOrEmpty(encryptedValue, nameof(encryptedValue));
            Guard.AgainstNullOrEmpty(key, nameof(key));
            Guard.AgainstNull(selectedEntityTypes, nameof(selectedEntityTypes));

            provider.ValidateEntityTypes(selectedEntityTypes);  
            foreach (var entityType in selectedEntityTypes)
            { 
                _entityTypes.Add(new SelectedEntityType(entityType)); 
            }
            CompanyId = companyId;
            ProviderId = providerId;
            Credential = new IntegrationCredential(key, encryptedValue);
            
        }
        public void AddCredential(string key, string encryptedValue)
        {
            Guard.AgainstNullOrEmpty(key, nameof(key));
            Guard.AgainstNullOrEmpty(encryptedValue, nameof(encryptedValue));

            if (Credential != null)
            throw new Exception("Credential already exists.");

            Credential = new IntegrationCredential(key, encryptedValue);

            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateCredential(string key, string encryptedValue)
        {
            Guard.AgainstNullOrEmpty(key, nameof(key));
            Guard.AgainstNullOrEmpty(encryptedValue, nameof(encryptedValue));

            if (Credential == null)
            { 
                throw new Exception("No credential exists.");
            }

            Credential = new IntegrationCredential(key, encryptedValue);

            UpdatedAt = DateTime.UtcNow;
        }
        public void RemoveCredential()
        {
            if (Credential == null)
            { 
                return; 
            }

            Credential = null;

            UpdatedAt = DateTime.UtcNow;
        }
        public void AddEntityType(IntegrationEntityType entityType)
        {
            
            Guard.AgainstNull(entityType, nameof(entityType));
            Guard.AgainstNull(Provider, nameof(Provider));

            if (!Provider.SupportsEntityType(entityType))       
                throw new Exception($"Provider does not support '{entityType}'.");

            if (_entityTypes.Any(e => e.EntityType == entityType))
                throw new Exception($"'{entityType}' is already activated.");

            _entityTypes.Add(new SelectedEntityType(entityType));
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveEntityType(IntegrationEntityType entityType)
        {
            Guard.AgainstNull(entityType, nameof(entityType));

            var existing =_entityTypes.FirstOrDefault(e => e.EntityType == entityType);
            if (existing == null)
                throw new Exception($"'{entityType}' is not activated.");

            _entityTypes.Remove(existing);
            UpdatedAt = DateTime.UtcNow;
        }
        public IntegrationMapping CreateMapping(IntegrationMappingBuilder builder)
        {
            Guard.AgainstNull(builder, nameof(builder));
            //rules
            var mapping = builder.WithSetting(this).Build();
            _mappings.Add(mapping);
            return mapping;
        }

        public void RemoveAllMappings()
        {
            foreach (var mapping in _mappings.Where(m => !m.IsDeleted))
            {
                mapping.SoftDelete();
            }
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
