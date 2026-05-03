using Domain.Guards;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity.Mapping
{
    public class IntegrationSetting : Base
    {
        public Guid CompanyId { get; private set; }
        public DataSource Provider { get; private set; } // F.eks. "Economic" eller "Dinero"
        public string Key { get; private set; }      // F.eks. "AgreementGrantToken"
        public string EncryptedValue { get; private set; }    // Selve token-strengen
        internal IntegrationSetting(Guid companyId, DataSource provider, string key, string encryptedValue) : base()
        {
            Guard.AgainstEmptyGuid(companyId, nameof(companyId));
            Guard.AgainstNullOrEmpty(encryptedValue, nameof(encryptedValue));
            Guard.AgainstNullOrEmpty(key, nameof(key));
            CompanyId = companyId;
            Provider = provider;
            Key = key;
            EncryptedValue = encryptedValue;
        }
        public void UpdateEncryptedValue(string newEncryptedValue)
        {
            Guard.AgainstNullOrEmpty(newEncryptedValue, nameof(newEncryptedValue));
            EncryptedValue = newEncryptedValue;
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateProvider(DataSource newProvider)
        {
            Provider = newProvider;
            UpdatedAt = DateTime.UtcNow;
        }
         public void UpdateKey(string newKey)
        {
            Guard.AgainstNullOrEmpty(newKey, nameof(newKey));
            Key = newKey;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
