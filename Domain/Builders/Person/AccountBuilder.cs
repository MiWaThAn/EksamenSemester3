using Domain.Entity.Person;
using Domain.Guards;
using Domain.Interfaces.Person;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Builders.Person
{
    public class AccountBuilder
    {
        internal string HashedPassword;
        internal string HashedPin;
        internal string Username;
        internal string PhoneNumber;
        public AccountBuilder WithHashedPassword(string hashedPassword)
        {
            Guard.AgainstNullOrEmpty(hashedPassword,nameof(hashedPassword));
            HashedPassword = hashedPassword;
            return this;
        }
        public AccountBuilder WithHashedPin(string hashedPin)
        {
            Guard.AgainstNullOrEmpty(hashedPin,nameof(hashedPin));
            HashedPin = hashedPin;
            return this;
        }
        public AccountBuilder WithUsername(string username)
        {
            Guard.AgainstNullOrEmpty(username, nameof(username));
            Username = username;
            return this;
        }
        public AccountBuilder WithPhoneNumber(string phoneNumber)
        {
            Guard.AgainstNullOrEmpty(phoneNumber, nameof(phoneNumber));
            PhoneNumber = phoneNumber;
            return this;
        }
        internal Account Build()
        {
            Guard.AgainstNullOrEmpty(Username, nameof(Username));
            Guard.AgainstNullOrEmpty(PhoneNumber, nameof(PhoneNumber));
            Guard.AgainstNullOrEmpty(HashedPassword, nameof(HashedPassword));
            Guard.AgainstNullOrEmpty(HashedPin, nameof(HashedPin));
            return new Account(Username, HashedPassword, PhoneNumber, HashedPin);
        }
    }
}
