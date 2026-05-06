using Domain.Builders.Item;
using Domain.Entity.Item.Activities;
using Domain.Entity.Item.Registrations;
using Domain.Entity.Mapping;
using Domain.Entity.Person;
using Domain.Guards;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain.Entity.Item
{
    public class Project : Base
    {
        public string Name { get; internal set; }
        public Status Status { get; internal set; }
        public Guid CompanyId { get; internal set; }
        public Guid? CustomerId { get; internal set; }
        public Guid? ResponsibleEmployeeId { get; internal set; }
        public Address? Address { get; internal set; }
        public string Description { get; internal set; } = string.Empty;

        private readonly List<ProjectActivity> _activities = new();
        public IReadOnlyCollection<ProjectActivity> Activities => _activities.Where(a => !a.IsDeleted).ToList().AsReadOnly();
        private readonly List<Registration> _registrations = new();
        public IReadOnlyCollection<Registration> Registrations => _registrations.Where(r => !r.IsDeleted).ToList().AsReadOnly();
        
        public Project() : base()
        {
        }
        internal Project(string name, Guid companyId, Guid? customerId, Guid? responsibleEmployeeId, string description,Status status, Address? address) : base()
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstEmptyGuid(companyId, nameof(companyId));
            Guard.AgainstNull(description, nameof(description));
            Name = name;
            CompanyId = companyId;
            CustomerId = customerId;
            ResponsibleEmployeeId = responsibleEmployeeId;
            Description = description;
            Status = status;
            Address = address;
        }
        public ProjectActivity CreateProjectActivity(ProjectActivityBuilder builder)
        {
            Guard.AgainstNull(builder, nameof(builder));
            var activity = builder.WithProject(this).Build();
            if (activity.ProjectId != this.Id) throw new ArgumentException("Projekt aktivite tilhører ikke dette projekt.");
            if (_activities.Exists(a => a.Id == activity.Id)) throw new ArgumentException("Denne projekt aktivitet er allerede i dette projekt.");
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
        public void LinkToEmployee(Employee employee)
        {
            Guard.AgainstNull(employee, nameof(employee));
            ResponsibleEmployeeId = employee.Id;
            UpdatedAt = DateTime.UtcNow;
        }
        public void LinkToCustomer(Customer customer)
        {
            Guard.AgainstNull(customer, nameof(customer));
            CustomerId = customer.Id;
            UpdatedAt = DateTime.UtcNow;
        }
        public void AddAddress(Address address)
        {
            Guard.AgainstNull(address, nameof(address));
            Address = address;
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateProjectName(string newName)
        {
            Guard.AgainstNullOrEmpty(newName, nameof(newName));
            Name = newName;
            UpdatedAt = DateTime.UtcNow;
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
        public void UpdateDescription(string newDescription)
        {
            Guard.AgainstNull(newDescription, nameof(newDescription));
            Description = newDescription;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
