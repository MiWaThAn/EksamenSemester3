using Domain.Entity.Item;
using Domain.Entity.Item.Activity;
using Domain.Entity.Person;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Builders.Item
{
    public class ProjectActivityBuilder
    {
        private Guid ActivityId;
        private string ActivityNumber;
        private Guid ProjectId;
        private DateTime StartDate;
        private DateTime EndDate;
        private bool IsCompleted;
        private Guid? ResponsibleEmployeeId;
        public ProjectActivityBuilder WithActivity(Activity activity)
        {
            ActivityId = activity.Id;
            ActivityNumber = activity.ActivityNumber;
            return this;
        }
        public ProjectActivityBuilder WithStartAndEndDates(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
            return this;
        }
        public ProjectActivityBuilder WithIsCompleted(bool isCompleted)
        {
            IsCompleted = isCompleted;
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
            if (ActivityId == Guid.Empty) throw new ArgumentException("Activity ID cannot be empty.");
            if (string.IsNullOrWhiteSpace(ActivityNumber)) throw new ArgumentException("Activity number cannot be null or whitespace.");
            if (ProjectId == Guid.Empty) throw new ArgumentException("Project ID cannot be empty.");
            if (StartDate == default) throw new ArgumentException("Start date must be set.");
            if (EndDate == default) throw new ArgumentException("End date must be set.");
            return new ProjectActivity(ActivityId, ActivityNumber, ProjectId, StartDate, EndDate, IsCompleted, ResponsibleEmployeeId);
        }
}
