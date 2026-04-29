using Domain.Entity.Item.Activity;
using Domain.Entity.Person;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Builders.Item
{
    public class ActivityBuilder
    {
        public string ActivityNumber { get; internal set; }
        public string Name { get; internal set; }
        public string Description { get; internal set; }
        public Guid CompanyId { get; internal set; }
        public ActivityBuilder WithActivityNumber(string activityNumber)
        {
            ActivityNumber = activityNumber ?? throw new ArgumentNullException(nameof(activityNumber));
            return this;
        }
        public ActivityBuilder WithName(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            return this;
        }
        public ActivityBuilder WithDescription(string description)
        {
            Description = description ?? throw new ArgumentNullException(nameof(description));
            return this;
        }
        internal ActivityBuilder WithCompany(Company company)
        {
            if (company == null) throw new ArgumentNullException(nameof(company));
            CompanyId = company.Id;
            return this;
        }
        internal Activity Build()
        {
            if (string.IsNullOrEmpty(ActivityNumber)) throw new InvalidOperationException("Activity number must be provided.");
            if (string.IsNullOrEmpty(Name)) throw new InvalidOperationException("Name must be provided.");
            if (string.IsNullOrEmpty(Description)) throw new InvalidOperationException("Description must be provided.");
            if (CompanyId == Guid.Empty) throw new InvalidOperationException("Company must be provided.");
            return new Activity(ActivityNumber, Name, Description) { CompanyId = CompanyId };
        }
    }
}
