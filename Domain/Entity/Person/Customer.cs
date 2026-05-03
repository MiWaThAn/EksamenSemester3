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
        public EmailAddress Email { get; internal set; }
        public string PhoneNumber { get; internal set; }


        internal Customer(string name, EmailAddress email, string phoneNumber) : base()
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNullOrEmpty(phoneNumber, nameof(phoneNumber));
            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
        }
        public void UpdateContactInfo(EmailAddress newEmail, string newPhoneNumber)
        {
            Guard.AgainstNullOrEmpty(newPhoneNumber, nameof(newPhoneNumber));
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