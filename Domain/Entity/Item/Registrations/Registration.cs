using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity.Item.Registrations
{
    public abstract class Registration : Base
    {
        public Guid EmployeeId { get; protected set; }
        public Guid ProjectId { get; protected set; }
        public Guid? ActivityId { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public string RegistrationNumber { get; protected set; }
        public string Description { get; protected set; } = string.Empty;
        public Status Status { get; protected set; }
        protected Registration(Guid employeeId, Guid projectId, Guid? activityId, string description, string registrationNumber) : base()
        {
            if (employeeId == Guid.Empty) throw new ArgumentException("Employee ID cannot be empty.");
            if (projectId == Guid.Empty) throw new ArgumentException("Project ID cannot be empty.");
            EmployeeId = employeeId;
            ProjectId = projectId;
            ActivityId = activityId;
            CreatedAt = DateTime.UtcNow;
            Description = description;
            RegistrationNumber = registrationNumber;
        }
        internal virtual void ValidateAgainst(IEnumerable<Registration> existingRegistrations)
        {
        }
    }
}
