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

        public Registration() : base()
        {

        }
        protected Registration(Guid projectId, WorkLog workLog, Guid? activityId, string? description, RegistrationStatus status) : base()
        {
            Guard.AgainstNull(projectId, nameof(projectId));
            Guard.AgainstNull(workLog, nameof(workLog));
            EmployeeId = workLog.EmployeeId;
            WorkLogId = workLog.Id;
            ProjectId = projectId;
            ProjectActivityId = activityId;
            if (description != null)
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
            MarkAsPending();
            UpdatedAt = DateTime.UtcNow;
        }
        internal void Approve(Company company)
        {
            Guard.AgainstNull(company, nameof(company));
            if (Status != RegistrationStatus.Pending)
                throw new InvalidOperationException("Kun afventende registreringer kan godkendes.");
            Status = RegistrationStatus.Godkendt;
            UpdatedAt = DateTime.UtcNow;
        }
        internal void Reject(Company company)
        {
            if (Status != RegistrationStatus.Pending)
                throw new InvalidOperationException("Kun afventende registreringer kan afvises.");
            Status = RegistrationStatus.Afvist;
            UpdatedAt = DateTime.UtcNow;
        }
        internal void MarkAsPending()
        {
            if (Status == RegistrationStatus.Afvist)
            {
                Status = RegistrationStatus.Pending;
                UpdatedAt = DateTime.UtcNow;
            }
        }
        public void LinkToActivity(Guid activityId)
        {
            Guard.AgainstEmptyGuid(activityId, nameof(activityId));
            ProjectActivityId = activityId;
            MarkAsPending();
            UpdatedAt = DateTime.UtcNow;
        }
        public void UnlinkFromActivity()
        {
            ProjectActivityId = null;
            MarkAsPending();
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
