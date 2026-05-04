using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain.ValueObjects
{
    [ComplexType]
    public record CvrNumber
    {
        public string Value { get; init; }
        public CvrNumber()
        {

        }
        public CvrNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("CVR-nummer må ikke være tomt.");

            // Domænereglen: Skal være præcis 8 cifre
            if (value.Length != 8 || !System.Text.RegularExpressions.Regex.IsMatch(value, @"^\d{8}$"))
                throw new ArgumentException("Et CVR-nummer skal bestå af præcis 8 cifre.");
            Value = value;
        }
        public override string ToString() => Value;
    }
}
