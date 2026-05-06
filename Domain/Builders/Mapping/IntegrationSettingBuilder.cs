using Domain.Entity.Mapping;
using Domain.Entity.Person;
using Domain.Guards;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Builders.Mapping
{
    public class IntegrationSettingBuilder
    {
        private Guid CompanyId;
        private DataSource Provider;
        private string Key;
        private string EncryptedValue;

        public IntegrationSettingBuilder WithCompany(Company company)
        {
            Guard.AgainstNull(company, nameof(company));
            CompanyId = company.Id;
            return this;
        }
        public IntegrationSettingBuilder WithProvider(DataSource provider)
        {
            Guard.AgainstNull(provider, nameof(provider));
            Provider = provider;
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
        internal IntegrationSetting Build()
        {
            return new IntegrationSetting(CompanyId, Provider, Key, EncryptedValue);
        }
    }
}
