using Domain.Guards;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity.Mapping.ValueObjects
{
    public sealed record DataSource
    {
        public string Value { get; }

        private DataSource(string value) => Value = value;
        public static DataSource From(string value)
        {
            Guard.AgainstNullOrEmpty(value, nameof(value));
            return new DataSource(value.Trim().ToLowerInvariant());
        }

        public override string ToString() => Value;
    }
}
