using Domain.Builders.Item;
using Domain.Entity.Item.Activity;
using Domain.Entity.Item.Registrations;
using Domain.Entity.Person;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity.Item
{
    public class Project : Base
    {
        public string ProjectNumber { get; internal set; }
        public string Name { get; internal set; }
        public Guid AdressId { get; internal set; }
        public Guid CompanyId { get; internal set; }
        public Guid? CustomerId { get; internal set; }
        public Guid? ResponsibleEmployeeId { get; internal set; }
        public bool IsClosed { get; internal set; }
        public bool IsDeleted { get; internal set; }
        public DateTime UpdatedAt { get; internal set; }
        public string Description { get; internal set; } = string.Empty;
        private readonly List<ProjectActivity> _activities = new();
        public IReadOnlyCollection<ProjectActivity> Activities => _activities.AsReadOnly();
        private readonly List<Registration> _registrations = new();
        public IReadOnlyCollection<Registration> Registrations => _registrations.AsReadOnly();
        internal Project(string projectNumber, string name, Guid adressId, Guid companyId, Guid? customerId, Guid? responsibleEmployeeId, string description) : base()
        {
            ProjectNumber = projectNumber ?? throw new ArgumentNullException(nameof(projectNumber));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            AdressId = adressId;
            CompanyId = companyId;
            CustomerId = customerId;
            ResponsibleEmployeeId = responsibleEmployeeId;
            IsClosed = false;
            IsDeleted = false;
            UpdatedAt = DateTime.UtcNow;
            Description = description;
        }
        public ProjectActivity CreateProjectActivity(ProjectActivityBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            var activity = builder.WithProject(this).Build();
            if (activity.ProjectId != this.Id) throw new ArgumentException("Project activity does not belong to this project.");
            if (_activities.Exists(a => a.Id == activity.Id)) throw new ArgumentException("This project activity is already added to the project.");
            _activities.Add(activity);
            UpdatedAt = DateTime.UtcNow;
            return activity;
        }

        public void RemoveProjectActivity(Guid activityId)
        {
            var activity = _activities.Find(a => a.Id == activityId);
            if (activity == null) throw new ArgumentException("Project activity not found for this project.");
            _activities.Remove(activity);
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateProjectName(string newName)
        {
            Name = newName ?? throw new ArgumentNullException(nameof(newName));
            UpdatedAt = DateTime.UtcNow;
        }
        public void MarkAsClosed()
        {
            IsClosed = true;
            UpdatedAt = DateTime.UtcNow;
        }
        public void MarkAsDeleted()
        {
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateDescription(string newDescription)
        {
            Description = newDescription ?? throw new ArgumentNullException(nameof(newDescription));
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
