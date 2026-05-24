using Domain.Builders.Person;
using Domain.Entity.Item;
using Domain.Entity.Item.Activities;
using Domain.Entity.Item.Registrations;
using Domain.Entity.Person;
using Domain.Guards;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Activity = Domain.Entity.Item.Activities.Activity;

namespace Domain.Builders.Item.Registration
{
    public abstract class RegistrationBuilder<TBuilder, TEntity> where TBuilder : RegistrationBuilder<TBuilder, TEntity>
    {
        protected WorkLog WorkLog;
        protected Guid? ActivityId;
        protected Guid ProjectId;
        protected RegistrationStatus Status = RegistrationStatus.Pending;
        protected string RegistrationNumber = string.Empty;
        protected string? Description = string.Empty;
        internal TBuilder WithWorkLog(WorkLog workLog)
        {
            Guard.AgainstNull(workLog, nameof(workLog));
            WorkLog = workLog;
            return (TBuilder)this;
        }
        public TBuilder WithProjectActivity(ProjectActivity activity)
        {
            Guard.AgainstNull(activity, nameof(activity));
            ActivityId = activity.Id;
            return (TBuilder)this;
        }
        internal TBuilder WithProjectActivity(Guid? activityId)
        {
            ActivityId = activityId;
            return (TBuilder)this;
        }
        public TBuilder WithProject(Project project)
        {
            Guard.AgainstNull(project, nameof(project));
            ProjectId = project.Id;
            return (TBuilder)this;
        }
        internal TBuilder WithProject(Guid projectid)
        {
            ProjectId = projectid;
            return (TBuilder)this;
        }
        public TBuilder WithDescription(string description)
        {
            Guard.AgainstNull(description, nameof(description));
            Description = description;
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
