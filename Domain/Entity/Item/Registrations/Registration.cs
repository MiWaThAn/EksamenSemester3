using Domain.Entity.Person;
using Domain.Guards;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Text;

namespace Domain.Entity.Item.Registrations
{
    public abstract class Registration : Base
    {
        [ForeignKey("Employee")]
        public Guid EmployeeId { get; protected set; }
        public Employee Employee { get; protected set; }
        [ForeignKey("Project")]
        public Guid ProjectId { get; protected set; }
        public Project Project { get; protected set; }
        [ForeignKey("Activity")]
        public Guid? ActivityId { get; protected set; }
        public Activity? Activity { get; protected set; }
        public string Description { get; protected set; }
        public RegistrationStatus Status { get; protected set; }

        public Registration() : base()
        {

        }
        protected Registration(Guid employeeId, Guid projectId, Guid? activityId, string description, RegistrationStatus status) : base()
        {
            Guard.AgainstEmptyGuid(employeeId, nameof(employeeId));
            Guard.AgainstEmptyGuid(projectId, nameof(projectId));
            EmployeeId = employeeId;
            ProjectId = projectId;
            ActivityId = activityId;
            Description = description;
            Status = status;
        }
        internal virtual void ValidateAgainst(IEnumerable<Registration> existingRegistrations)
        {
        }
        public void UpdateDescription(string newDescription)
        {
            Guard.AgainstNull(newDescription, nameof(newDescription));
            Description = newDescription;
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
            ActivityId = activityId;
            UpdatedAt = DateTime.UtcNow;
        }
        public void UnlinkFromActivity()
        {
            ActivityId = null;
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateProject(Guid newProjectId)
        {
            Guard.AgainstEmptyGuid(newProjectId, nameof(newProjectId));
            ProjectId = newProjectId;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
