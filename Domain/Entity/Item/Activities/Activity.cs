using Domain.Entity.Mapping;
using Domain.Guards;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain.Entity.Item.Activities
{
    public class Activity : Base
    {
        public string Name { get; internal set; }
        public string Description { get; internal set; }
        [ForeignKey("Company")]
        public Guid CompanyId { get; internal set; }

        public Activity() : base()
        {

        }
        internal Activity(string name, string description, Guid companyId) : base()
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNull(description, nameof(description));
            Guard.AgainstEmptyGuid(companyId, nameof(companyId));
            Name = name;
            Description = description;
            CompanyId = companyId;
        }
        public void UpdateActivityName(string newName)
        {
            Guard.AgainstNullOrEmpty(newName, nameof(newName));
            Name = newName;
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateActivityDescription(string newDescription)
        {
            Guard.AgainstNull(newDescription, nameof(newDescription));
            Description = newDescription;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
