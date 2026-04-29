using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity.Item.Activity
{
    public class Activity : Base
    {
        public string ActivityNumber { get; internal set; }
        public string Name { get; internal set; }
        public string Description { get; internal set; }
        public DateTime UpdatedAt { get; internal set; }
        public Guid CompanyId { get; internal set; }
        internal Activity(string activityNumber, string name, string description) : base()
        {
            ActivityNumber = activityNumber ?? throw new ArgumentNullException(nameof(activityNumber));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateActivityName(string newName)
        {
            Name = newName ?? throw new ArgumentNullException(nameof(newName));
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateActivityDescription(string newDescription)
        {
            Description = newDescription ?? throw new ArgumentNullException(nameof(newDescription));
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
