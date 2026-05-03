using Domain.Builders.Person;
using Domain.Entity.Item;
using Domain.Entity.Item.Activity;
using Domain.Entity.Item.Registrations;
using Domain.Entity.Person;
using Domain.Guards;
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
        protected RegistrationStatus Status = RegistrationStatus.Pending;
        protected string RegistrationNumber = string.Empty;
        protected string Description = string.Empty;
        public TBuilder WithProject(Project project)
        {
            Guard.AgainstNull(project, nameof(project));
            ProjectId = project.Id;
            return (TBuilder)this;
        }
        public TBuilder WithActivity(Activity activity)
        {
            Guard.AgainstNull(activity, nameof(activity));
            ActivityId = activity.Id;
            return (TBuilder)this;
        }
        public TBuilder WithDescription(string description)
        {
            Guard.AgainstNull(description, nameof(description));
            Description = description;
            return (TBuilder)this;
        }
        internal TBuilder WithEmployee(Employee employee)
        {
            Guard.AgainstNull(employee, nameof(employee));
            EmployeeId = employee.Id;
            return (TBuilder)this;
        }
        public TBuilder WithStatus(RegistrationStatus status)
        {
            Status = status;
            return (TBuilder)this;
        }
        internal abstract TEntity Build();
    }
}
