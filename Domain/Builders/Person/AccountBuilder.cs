using Domain.Entity.Person;
using Domain.Guards;
using Domain.Interfaces.Person;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Builders.Person
{
    public class AccountBuilder
    {
        internal string HashedPassword;
        internal string? HashedPin;
        internal string Username;
        internal PhoneNumber PhoneNumber;
        internal Company? Company;
        internal Employee? Employee;
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
        public AccountBuilder WithCompany(Company company)
        {
            Guard.AgainstNull(company, nameof(company));
            Company = company;
            return this;
        }
        public AccountBuilder WithEmployee(Employee employee)
        {
            Guard.AgainstNull(employee, nameof(employee));
            Employee = employee;
            return this;
        }
        public AccountBuilder WithUsername(string username)
        {
            Guard.AgainstNullOrEmpty(username, nameof(username));
            Username = username;
            return this;
        }
        public AccountBuilder WithPhoneNumber(PhoneNumber phoneNumber)
        {
            Guard.AgainstNull(phoneNumber, nameof(phoneNumber));
            PhoneNumber = phoneNumber;
            return this;
        }
        internal Account Build()
        {
            Guard.AgainstNullOrEmpty(Username, nameof(Username));
            Guard.AgainstNull(PhoneNumber, nameof(PhoneNumber));
            Guard.AgainstNullOrEmpty(HashedPassword, nameof(HashedPassword));
            return new Account(Username, HashedPassword, PhoneNumber, HashedPin,Employee,Company);
        }
    }
}
