using Domain.Guards;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects
{
    [ComplexType]
    public record Pincode
    {
        public string Value { get; init; }
        public Pincode()
        {
            //lav pinkode model med required og length
            //lav set pincode metode i account
            //når pinkode skal sættes eller opdaterers eller konto skal forbindes med en medarbejder skal password med requestet
        }
        public Pincode(string code)
        {

            Guard.AgainstNullOrEmpty(code, nameof(code));

            if (!Regex.IsMatch(code, @"[1-9]\d{4,8}$"))
            {
                throw new ArgumentException("Ugyldigt pinkode format", nameof(code));
            }

            Value = code;
        }
    }
}
