using Domain.Entity.Item.Activities;
using Domain.Entity.Person;
using Domain.Guards;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Builders.Item
{
    public class ActivityBuilder
    {
        private string Name;
        private string Description;
        private Guid CompanyId;
        public ActivityBuilder WithName(string name)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Name = name;
            return this;
        }
        public ActivityBuilder WithDescription(string description)
        {
            Guard.AgainstNullOrEmpty(description, nameof(description));
            Description = description;
            return this;
        }
        internal ActivityBuilder WithCompany(Company company)
        {
            Guard.AgainstNull(company, nameof(company));
            CompanyId = company.Id;
            return this;
        }
        internal Activity Build()
        {
            Guard.AgainstNullOrEmpty(Description, nameof(Description));
            Guard.AgainstNullOrEmpty(Name, nameof(Name));
            Guard.AgainstEmptyGuid(CompanyId, nameof(CompanyId));
            return new Activity(Name, Description, CompanyId);
        }
    }
}
