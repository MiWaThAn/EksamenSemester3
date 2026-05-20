using Domain.Entity.Item.Activities;
using Domain.Entity.Person;
using Domain.Guards;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Text;

namespace Domain.Entity.Item.Registrations
{
    public class Registration : Base
    {
        public Guid EmployeeId { get; protected set; }
        public Guid ProjectId { get; protected set; }
        public Guid WorkLogId { get; protected set; }
        public Guid? ProjectActivityId { get; protected set; }
        public string Description { get; protected set; } = "";
        public RegistrationStatus Status { get; protected set; }
        public bool IsBreak => ProjectActivityId == null;

        public Registration() : base()
        {

        }
        protected Registration(WorkLog workLog, Guid? activityId, string? description, RegistrationStatus status) : base()
        {
            Guard.AgainstNull(workLog, nameof(workLog));
            WorkLogId = workLog.Id;
            EmployeeId = workLog.EmployeeId;
            ProjectId = workLog.ProjectId;
            ProjectActivityId = activityId;
            if(description != null)
                Description = description;
            Status = status;
        }
        internal virtual void ValidateAgainst(IEnumerable<Registration> existingRegistrations)
        {
        }
        public void UpdateDescription(string? newDescription)
        {
            if (newDescription != null)
                Description = newDescription;
            Description = "";
            UpdatedAt = DateTime.UtcNow;
        }
        public void MarkAsApproved()
        {
            Status = RegistrationStatus.Godkendt;
            UpdatedAt = DateTime.UtcNow;
        }
        public void MarkAsRejected()
        {
            Status = RegistrationStatus.Afvist;
            UpdatedAt = DateTime.UtcNow;
        }
        public void MarkAsPending()
        {
            Status = RegistrationStatus.Pending;
            UpdatedAt = DateTime.UtcNow;
        }
        public void LinkToActivity(Guid activityId)
        {
            Guard.AgainstEmptyGuid(activityId, nameof(activityId));
            ProjectActivityId = activityId;
            UpdatedAt = DateTime.UtcNow;
        }
        public void UnlinkFromActivity()
        {
            ProjectActivityId = null;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
