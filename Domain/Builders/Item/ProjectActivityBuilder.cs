using Domain.Entity.Item;
using Domain.Entity.Item.Activities;
using Domain.Entity.Person;
using Domain.Guards;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Builders.Item
{
    public class ProjectActivityBuilder
    {
        private Guid ActivityId;
        private Guid ProjectId;
        private DateTime StartDate;
        private DateTime EndDate;
        private Status Status;
        private Guid? ResponsibleEmployeeId;
        public ProjectActivityBuilder WithActivity(Activity activity)
        {
            ActivityId = activity.Id;
            return this;
        }
        public ProjectActivityBuilder WithStartAndEndDates(DateTime startDate, DateTime endDate)
        {
            Guard.AgainstInvalidTimeRange(startDate, endDate);
            StartDate = startDate;
            EndDate = endDate;
            return this;
        }
        public ProjectActivityBuilder WithStatus(Status status)
        {
            Status = status;
            return this;
        }
        public ProjectActivityBuilder WithResponsibleEmployee(Employee employee)
        {
            ResponsibleEmployeeId = employee.Id;
            return this;
        }
        internal ProjectActivityBuilder WithProject(Project project)
        {
            ProjectId = project.Id;
            return this;
        }
        internal ProjectActivity Build()
        {
            Guard.AgainstEmptyGuid(ProjectId, nameof(ProjectId));
            Guard.AgainstEmptyGuid(ActivityId, nameof(ActivityId));
            Guard.AgainstInvalidTimeRange(StartDate,EndDate);
            return new ProjectActivity(ActivityId, ProjectId, StartDate, EndDate, ResponsibleEmployeeId, Status);
        }
    }
}
