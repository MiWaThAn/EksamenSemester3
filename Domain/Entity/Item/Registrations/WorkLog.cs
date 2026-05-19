using Domain.Builders.Item.Registration;
using Domain.Entity.Person;
using Domain.Guards;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity.Item.Registrations
{
    public class WorkLog : Base
    {
        //Worklog needs a builder that is given to the relevant employee. It then needs to check if theres already a worklog in their worklogs that overlaps with the current one
        public Guid EmployeeId { get; private set; }
        public Guid ProjectId { get; private set; }
        public DateTime DateCreated { get; private set; }

        private readonly List<Registration> _registrations = new();
        public IReadOnlyCollection<Registration> ActiveRegistrations => _registrations.Where(r => !r.IsDeleted).ToList().AsReadOnly();

        internal WorkLog(Employee employee, Project project)
        {
            Guard.AgainstNull(employee, nameof(employee));
            Guard.AgainstNull(project, nameof(project));
            EmployeeId = employee.Id;
            ProjectId = project.Id;
            DateCreated = DateTime.UtcNow;
            project.AddWorkLog(this);
        }

        // --- Business Methods (The UI Buttons) ---
        public void StartTrack() { /* ... */ }
        public void TakeBreak() { /* ... */ }
        public void SwitchActivity(Guid newActivityId) { /* ... */ }

        public TEntity CreateRegistration<TBuilder, TEntity>(RegistrationBuilder<TBuilder, TEntity> builder) where TBuilder : RegistrationBuilder<TBuilder, TEntity> where TEntity : Registration
        {
            Guard.AgainstNull(builder, nameof(builder));
            var registration = builder.WithWorkLog(this).Build();
            if (registration.WorkLogId != this.Id) throw new ArgumentException("Denne registrering tilhører ikke denne log");
            registration.ValidateAgainst(_registrations);
            _registrations.Add(registration);
            UpdatedAt = DateTime.UtcNow;
            return registration;
        }
        public WorkLog() { }
    }
}
