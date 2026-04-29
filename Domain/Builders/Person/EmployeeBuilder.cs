using Domain.Entity.Item.Registrations;
using Domain.Entity.Person;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Builders.Person
{
    public class EmployeeBuilder : AccountBuilder<EmployeeBuilder, Employee>
    {
        private string EmployeeNumber;
        private bool Autonomous;
        private string HashedPin;
        private EmployeeType EmployeeType;
        private Guid CompanyId;
        public EmployeeBuilder WithEmployeeNumber(string employeeNumber)
        {
            EmployeeNumber = employeeNumber ?? throw new ArgumentNullException(nameof(employeeNumber));
            return this;
        }
        public EmployeeBuilder WithAustonomy(bool autonomous)
        {
            Autonomous = autonomous;
            return this;
        }
        public EmployeeBuilder WithHashedPin(string hashedPin)
        {
            HashedPin = hashedPin ?? throw new ArgumentNullException(nameof(hashedPin));
            return this;
        }
        public EmployeeBuilder WithEmployeeType(EmployeeType employeeType)
        {
            EmployeeType = employeeType;
            return this;
        }
        internal EmployeeBuilder WithCompany(Company company)
        {
            if (company == null) throw new ArgumentNullException(nameof(company));
            CompanyId = company.Id;
            return this;
        }
        internal Employee Build()
        {
            if (string.IsNullOrEmpty(Name)) throw new InvalidOperationException("Name is required to build an employee.");
            if (string.IsNullOrEmpty(HashedPassword)) throw new InvalidOperationException("Hashed password is required to build an employee.");
            if (string.IsNullOrEmpty(Username)) throw new InvalidOperationException("Username is required to build an employee.");
            if (string.IsNullOrEmpty(EmployeeNumber)) throw new InvalidOperationException("Employee number is required to build an employee.");
            if (CompanyId == Guid.Empty) throw new InvalidOperationException("Company ID is required to build an employee.");
            if (string.IsNullOrEmpty(HashedPin)) throw new InvalidOperationException("Hashed pin is required to build an employee.");
            return new Employee(Username, HashedPassword, Name, Email, PhoneNumber, EmployeeNumber, CompanyId, Autonomous, EmployeeType, HashedPin);
    }
}
