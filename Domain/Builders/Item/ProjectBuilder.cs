using Domain.Entity.Item;
using Domain.Entity.Person;
using Domain.Guards;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Builders.Item
{
    public class ProjectBuilder
    {
        private string Name; //
        private Address? Address; //
        private Guid CompanyId; //
        private Guid? CustomerId; //
        private Guid? ResponsibleEmployeeId; //
        private Status Status; //
        private string Description = string.Empty;
        public ProjectBuilder WithName(string name)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Name = name;
            return this;
        }
        public ProjectBuilder WithAddress(Address address)
        {
            Address = address;
            return this;
        }
        public ProjectBuilder WithCustomer(Customer customer)
        {
            CustomerId = customer.Id;
            return this;
        }
        public ProjectBuilder WithResponsibleEmployee(Employee responsibleEmployee)
        {
            ResponsibleEmployeeId = responsibleEmployee.Id;
            return this;
        }
        public ProjectBuilder WithIsStatus(Status status)
        {
            Status = status;
            return this;
        }
        public ProjectBuilder WithDescription(string description)
        {
            Guard.AgainstNull(description, nameof(description));
            Description = description;
            return this;
        }
        internal ProjectBuilder WithCompany(Company company)
        {
            if (company == null) throw new ArgumentNullException(nameof(company));
            CompanyId = company.Id;
            return this;
        }
        internal ProjectBuilder WithCompanyId(Guid companyId)
        {
            Guard.AgainstEmptyGuid(companyId, nameof(companyId));
            CompanyId = companyId;
            return this;
        }
        internal Project Build()
        {
            Guard.AgainstNullOrEmpty(Name, nameof(Name));
            Guard.AgainstEmptyGuid(CompanyId, nameof(CompanyId));
            return new Project(Name, CompanyId, CustomerId, ResponsibleEmployeeId, Description, Status,Address);
        }
    }
}
