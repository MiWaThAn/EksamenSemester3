using Domain.Guards;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain.Entity.Mapping.ValueObjects
{
    [ComplexType]
    public class IntegrationCredential
    {
        public string Key { get; } // F.eks. "AgreementGrantToken"
        public string Value { get; }  // Selve token-strengen
        
        internal IntegrationCredential(string key, string value)
        {
            Guard.AgainstNullOrEmpty(key, nameof(key));
            Guard.AgainstNullOrEmpty(value, nameof(value));

            Key = key;
            Value = value;
        }




    }
}
