using Domain.Entity.Item.Registrations;
using Domain.Entity.Mapping;
using Domain.Guards;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity.Item.Activity
{
    public class ProjectActivity : Base
    {
        public Guid ActivityId { get; internal set; }
        public Guid ProjectId { get; internal set; }
        public Status Status { get; internal set; }
        public DateTime StartDate { get; internal set; }
        public DateTime EndDate { get; internal set; }
        public Guid? ResponsibleEmployeeId { get; internal set; }
        private readonly List<Registration> _registrations = new();
        public IReadOnlyCollection<Registration> Registrations => _registrations.Where(r=>!r.IsDeleted).ToList().AsReadOnly();
        public ProjectActivity(Guid activityId, Guid projectId, DateTime startDate, DateTime endDate, Guid? responsibleEmployeeId, Status status) : base()
        {
            Guard.AgainstEmptyGuid(projectId, nameof(projectId));
            Guard.AgainstEmptyGuid(activityId, nameof(activityId));
            Guard.AgainstInvalidTimeRange(startDate, endDate);
            ActivityId = activityId;
            ProjectId = projectId;
            StartDate = startDate;
            EndDate = endDate;
            ResponsibleEmployeeId = responsibleEmployeeId;
            Status = status;
        }
        public void MarkAsClosed()
        {
            Status = Status.Lukket;
            UpdatedAt = DateTime.UtcNow;
        }
        public void MarkAsOpen()
        {
            Status = Status.Åben;
            UpdatedAt = DateTime.UtcNow;
        }
        public void MarkAsOnHold()
        {
            Status = Status.Godkendes;
            UpdatedAt = DateTime.UtcNow;
        }
        public void AssignResponsibleEmployee(Guid employeeId)
        {
            Guard.AgainstEmptyGuid(employeeId, nameof(employeeId));
            ResponsibleEmployeeId = employeeId;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
