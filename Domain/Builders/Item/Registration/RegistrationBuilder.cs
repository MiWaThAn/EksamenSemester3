using Domain.Builders.Person;
using Domain.Entity.Item;
using Domain.Entity.Item.Activity;
using Domain.Entity.Item.Registrations;
using Domain.Entity.Person;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Activity = Domain.Entity.Item.Activity.Activity;

namespace Domain.Builders.Item.Registration
{
    public abstract class RegistrationBuilder<TBuilder, TEntity> where TBuilder : RegistrationBuilder<TBuilder, TEntity>
    {
        protected Guid EmployeeId;
        protected Guid ProjectId;
        protected Guid? ActivityId;
        protected Status Status;
        protected string RegistrationNumber;
        protected string Description = string.Empty;
        public TBuilder WithProject(Project project)
        {
            ProjectId = project.Id == Guid.Empty ? throw new ArgumentException("Project ID cannot be empty.") : project.Id;
            return (TBuilder)this;
        }
        public TBuilder WithActivity(Activity activity)
        {
            ActivityId = activity.Id == Guid.Empty ? throw new ArgumentException("Activity ID cannot be empty.") : activity.Id;
            return (TBuilder)this;
        }
        public TBuilder WithDescription(string description)
        {
            Description = description ?? throw new ArgumentNullException(nameof(description), "Description cannot be null.");
            return (TBuilder)this;
        }
        internal TBuilder WithEmployee(Employee employee)
        {
            EmployeeId = employee.Id == Guid.Empty ? throw new ArgumentException("Employee ID cannot be empty.") : employee.Id;
            return (TBuilder)this;
        }
        public TBuilder WithStatus(Status status)
        {
            Status = status;
            return (TBuilder)this;
        }
        public TBuilder WithRegistrationNumber(string registrationNumber)
        {
            RegistrationNumber = registrationNumber;
            return (TBuilder)this;
        }
        internal abstract TEntity Build();
    }
}
