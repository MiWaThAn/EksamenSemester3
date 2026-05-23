using Domain.Guards;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity.Mapping.ValueObjects
{
    public sealed record IntegrationEntityType
    {
        public string Value { get; }
        public int SyncPriority { get; init; }

        private IntegrationEntityType(string value, int syncPriority)
        {
            Value = value;
            SyncPriority = syncPriority;
        }

        public static IntegrationEntityType From(string value, int syncPriority = 0)
        {
            Guard.AgainstNullOrEmpty(value, nameof(value));
            return new IntegrationEntityType(value.Trim().ToLowerInvariant(), syncPriority);
        }

        public override string ToString() => Value;
    }
}
