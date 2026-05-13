using Domain.Entity.Mapping;
using Domain.Entity.Person;
using Domain.Guards;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity.Mapping.ValueObjects;
namespace Domain.Builders.Mapping
{
    public class IntegrationSettingBuilder
    {
        private Guid CompanyId;
        private Guid ProviderId;
        private Provider Provider;
        private string Key;
        private string EncryptedValue;
        private List<IntegrationEntityType> SelectedEntityTypes = new();
        public IntegrationSettingBuilder WithCompany(Company company)
        {
            Guard.AgainstNull(company, nameof(company));
            CompanyId = company.Id;
            return this;
        }
        public IntegrationSettingBuilder WithProvider(Provider provider)
        {
            Guard.AgainstNull(provider, nameof(provider));
            Provider = provider;
            ProviderId = provider.Id;
            return this;
        }
        public IntegrationSettingBuilder WithKey(string key)
        {
            Guard.AgainstNullOrEmpty(key, nameof(key));
            Key = key;
            return this;
        }
        public IntegrationSettingBuilder WithEncryptedValue(string encryptedValue)
        {
            Guard.AgainstNullOrEmpty(encryptedValue, nameof(encryptedValue));
            EncryptedValue = encryptedValue;
            return this;
        }
        public IntegrationSettingBuilder WithIntegrationEntityTypes(List<IntegrationEntityType> entityTypes)
        {
            Guard.AgainstNull(entityTypes, nameof(entityTypes));
            SelectedEntityTypes = entityTypes;
            return this;
        }
        internal IntegrationSetting Build()
        {
            return new IntegrationSetting(
            CompanyId,
            ProviderId,
            SelectedEntityTypes,
            Provider,
            Key,
            EncryptedValue);
        }
    }
}
