using Domain.Guards;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity.Person
{
    public class Customer : Base
    {
        public string Name { get; internal set; }
        public EmailAddress? Email { get; internal set; }
        public PhoneNumber? PhoneNumber { get; internal set; }

        public Customer() : base()
        {

        }

        internal Customer(string name, EmailAddress? email, PhoneNumber? phoneNumber) : base()
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            
            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
        }
        public void UpdateContactInfo(EmailAddress newEmail, PhoneNumber? newPhoneNumber)
        {
            
            Email = newEmail;
            PhoneNumber = newPhoneNumber;
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateName(string newName)
        {
            Guard.AgainstNullOrEmpty(newName, nameof(newName));
            Name = newName;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}