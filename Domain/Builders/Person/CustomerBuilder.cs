using Domain.Entity.Person;
using Domain.Guards;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Builders.Person
{
    public class CustomerBuilder
    {
        private string Name;
        private EmailAddress? Email;
        private PhoneNumber? PhoneNumber;
        public CustomerBuilder WithName(string name)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Name = name;
            return this;
        }
        public CustomerBuilder WithEmail(EmailAddress? email)
        {
            
            Email = email;
            return this;
        }
        public CustomerBuilder WithPhoneNumber(PhoneNumber phoneNumber)
        {
            Guard.AgainstNull(phoneNumber, nameof(phoneNumber));
            PhoneNumber = phoneNumber;
            return this;
        }
        public Customer Build()
        {
            Guard.AgainstNullOrEmpty(Name, nameof(Name));
            return new Customer(Name, Email, PhoneNumber);
        }
    }
}
