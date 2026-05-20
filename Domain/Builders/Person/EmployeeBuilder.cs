using Domain.Entity.Item.Registrations;
using Domain.Entity.Person;
using Domain.Guards;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Builders.Person
{
    public class EmployeeBuilder
    {
        private string Name;
        private Guid CompanyId;
        private EmployeeType EmployeeType;
        private EmailAddress Email;
        private bool IsAutonomous;
        public EmployeeBuilder WithAutonomy(bool autonomous)
        {
            IsAutonomous = autonomous;
            return this;
        }
        public EmployeeBuilder WithName(string name)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Name = name;
            return this;
        }
        public EmployeeBuilder WithEmployeeType(EmployeeType employeeType)
        {
            EmployeeType = employeeType;
            return this;
        }
        public EmployeeBuilder WithEmail(EmailAddress? email)
        {
            Email = email;
            return this;
        }
        public EmployeeBuilder WithCompany(Company company)
        {
            if (company == null) throw new ArgumentNullException(nameof(company));
            CompanyId = company.Id;
            return this;
        }
        public EmployeeBuilder WithCompanyId(Guid companyId)
        {
            Guard.AgainstEmptyGuid(companyId, nameof(companyId));
            CompanyId = companyId;
            return this;
        }
        public Employee Build()
        {
            Guard.AgainstNullOrEmpty(Name, nameof(Name));
            Guard.AgainstEmptyGuid(CompanyId, nameof(CompanyId));
            return new Employee(Name, CompanyId, EmployeeType, Email, IsAutonomous);
        }
    }
}
