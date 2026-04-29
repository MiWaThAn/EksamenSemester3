using Domain.Entity.Item;
using Domain.Entity.Person;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Builders.Item
{
    public class ProjectBuilder
    {
        private string ProjectNumber; //
        private string Name; //
        private Guid AdressId; //
        private Guid CompanyId; //
        private Guid? CustomerId; //
        private Guid? ResponsibleEmployeeId; //
        private bool IsClosed; //
        private string Description; //
        public ProjectBuilder WithProjectNumber(string projectNumber)
        {
            ProjectNumber = projectNumber ?? throw new ArgumentNullException(nameof(projectNumber));
            return this;
        }
        public ProjectBuilder WithName(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            return this;
        }
        public ProjectBuilder WithAddress(Address address)
        {
            AdressId = address.Id;
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
        public ProjectBuilder WithIsClosed(bool isClosed)
        {
            IsClosed = isClosed;
            return this;
        }
        public ProjectBuilder WithDescription(string description)
        {
            Description = description ?? throw new ArgumentNullException(nameof(description));
            return this;
        }
        internal ProjectBuilder WithCompany(Company company)
        {
            if (company == null) throw new ArgumentNullException(nameof(company));
            CompanyId = company.Id;
            return this;
        }
        internal Project Build()
        {
            if (string.IsNullOrEmpty(ProjectNumber)) throw new InvalidOperationException("Project number is required to build a project.");
            if (string.IsNullOrEmpty(Name)) throw new InvalidOperationException("Name is required to build a project.");
            if (AdressId == Guid.Empty) throw new InvalidOperationException("Address ID is required to build a project.");
            if (CompanyId == Guid.Empty) throw new InvalidOperationException("Company ID is required to build a project.");
            return new Project(
                ProjectNumber, 
                Name, 
                AdressId, 
                CompanyId, 
                CustomerId ?? Guid.Empty, 
                ResponsibleEmployeeId ?? Guid.Empty, 
                Description);
        }
    }
}
