using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.ValueObjects
{
    public record EmailAddress
    {
        public string Value { get; init; }
        public EmailAddress(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email må ikke være tom.");

            if (!value.Contains("@") || !value.Contains("."))
                throw new ArgumentException("Ugyldigt email-format.");

            Value = value.ToLower().Trim(); 
        }

    }
}
