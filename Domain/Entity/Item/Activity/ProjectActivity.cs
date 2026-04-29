using Domain.Entity.Item.Registrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity.Item.Activity
{
    public class ProjectActivity : Base
    {
        public Guid ActivityId { get; internal set; }
        public string ActivityNumber { get; internal set; }
        public Guid ProjectId { get; internal set; }
        public DateTime UpdatedAt { get; internal set; }
        public DateTime StartDate { get; internal set; }
        public DateTime EndDate { get; internal set; }
        public bool IsCompleted { get; internal set; }
        public Guid? ResponsibleEmployeeId { get; internal set; }
        private readonly List<Registration> _registrations = new();
        public IReadOnlyCollection<Registration> Registrations => _registrations.AsReadOnly();
        public ProjectActivity(Guid activityId, string activityNumber, Guid projectId, DateTime startDate, DateTime endDate, bool isCompleted, Guid? responsibleEmployeeId) : base()
        {
            ActivityId = activityId == Guid.Empty ? throw new ArgumentNullException(nameof(activityId)) : activityId;
            ActivityNumber = activityNumber ?? throw new ArgumentNullException(nameof(activityNumber));
            ProjectId = projectId == Guid.Empty ? throw new ArgumentNullException(nameof(projectId)) : projectId;
            if(startDate >= endDate) throw new ArgumentException("Start date must be before end date.");
            if(startDate > DateTime.UtcNow) throw new ArgumentException("Start date cannot be in the future.");
            if(endDate > DateTime.UtcNow) throw new ArgumentException("End date cannot be in the future.");
            StartDate = startDate;
            EndDate = endDate;
            IsCompleted = isCompleted;
            ResponsibleEmployeeId = responsibleEmployeeId;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
