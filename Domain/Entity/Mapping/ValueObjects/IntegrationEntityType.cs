using Domain.Guards;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity.Mapping.ValueObjects
{
    public sealed record IntegrationEntityType
    {
        public string Value { get; }

        
        private IntegrationEntityType(string value) => Value = value;

        public static IntegrationEntityType From(string value)
        {
            Guard.AgainstNullOrEmpty(value, nameof(value));
            return new IntegrationEntityType(value.Trim().ToLowerInvariant());
        }

        public override string ToString() => Value;
    }
}
