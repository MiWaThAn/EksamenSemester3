using Domain.Guards;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects
{
    [ComplexType]
    public record PhoneNumber
    {
        public string Value { get; init; }
        public PhoneNumber()
        {

        }
        public PhoneNumber(string value)
        {

            Guard.AgainstNullOrEmpty(value, nameof(value));

            if (!Regex.IsMatch(value, @"^\+?[1-9]\d{1,14}$"))
            {
                throw new ArgumentException("Ugyldigt telefonnummer format", nameof(value));
            }

            Value = value;
        }

    }
}
