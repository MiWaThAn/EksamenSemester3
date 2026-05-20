using Domain.Builders.Item.Registration;
using Domain.Entity.Item.Activities;
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
        public Guid? ActiveRegistrationId { get; private set; }

        private readonly List<Registration> _registrations = new();
        public IReadOnlyCollection<Registration> Registrations => _registrations.Where(r => !r.IsDeleted).ToList().AsReadOnly();

        internal WorkLog(Employee employee, Project project)
        {
            Guard.AgainstNull(employee, nameof(employee));
            Guard.AgainstNull(project, nameof(project));
            EmployeeId = employee.Id;
            ProjectId = project.Id;
            DateCreated = DateTime.UtcNow;
            project.AddWorkLog(this);
        }

        //Business Methods (UI) 
        //Method for when the user wants to start work on an activity
        public void StartWork(ProjectActivity activity,string? description) 
        {
            Guard.AgainstNull(activity, nameof(activity));
            if(ActiveRegistrationId != null && ActiveRegistrationId!=Guid.Empty)
                throw new InvalidOperationException("Der er allerede en aktiv registrering.");
            var safeDescription = description ?? string.Empty;
            var builder = new HourRegistrationBuilder()
            .WithProjectActivity(activity)
            .WithDescription(safeDescription)
            .WithStart(DateTime.UtcNow)
            .WithType(TimeType.Work);

            ActiveRegistrationId = CreateRegistration(builder).Id;
            UpdatedAt = DateTime.UtcNow;
        }
        //metode til når en bruger vil have en pause
        public void TakeBreak() 
        {
            var active = GetActiveHourRegistration();
            active.SetEndTime(DateTime.UtcNow);
            var builder = new HourRegistrationBuilder()
            .WithDescription("Pause")
            .WithType(TimeType.Break)
            .WithStart(DateTime.UtcNow);

            ActiveRegistrationId = CreateRegistration(builder).Id;
            UpdatedAt = DateTime.UtcNow;
        }
        //metode til når en bruger fil forsætte arbejde
        public void ResumeWork(ProjectActivity activity, string? description)
        {
            var activeBreak = GetActiveHourRegistration();
            if (activeBreak.ProjectActivityId != null && activeBreak.Type!=TimeType.Break)
                throw new InvalidOperationException("Du er ikke på pause.");
            activeBreak.SetEndTime(DateTime.UtcNow);
            var safeDescription = description ?? string.Empty;
            var builder = new HourRegistrationBuilder()
                .WithProjectActivity(activity)
                .WithDescription(safeDescription)
                .WithStart(DateTime.UtcNow)
                .WithType(TimeType.Work);

            ActiveRegistrationId = CreateRegistration(builder).Id;
            UpdatedAt = DateTime.UtcNow;
        }
        //Metode til når en medarbejder vil skifte opgave
        public void SwitchActivity(ProjectActivity newActivity, string? newDescription) 
        {
            var active = GetActiveHourRegistration();

            active.SetEndTime(DateTime.UtcNow);
            var safeDescription = newDescription ?? string.Empty;
            var builder = new HourRegistrationBuilder()
                .WithProjectActivity(newActivity)
                .WithDescription(safeDescription)
                .WithStart(DateTime.UtcNow)
                .WithType(TimeType.Work);

            ActiveRegistrationId = CreateRegistration(builder).Id;
            UpdatedAt = DateTime.UtcNow;
        }
        //metode for at stoppe arbejde.
        public void EndWork()
        {
            var active = GetActiveHourRegistration();
            active.SetEndTime(DateTime.UtcNow);
            ActiveRegistrationId = null;
            UpdatedAt = DateTime.UtcNow;
        }
        private HourRegistration GetActiveHourRegistration()
        {
            var active = Registrations.OfType<HourRegistration>().FirstOrDefault(r => !r.IsFinished);
            if (active == null)
                throw new InvalidOperationException("Ingen aktiv registrering fundet.");
            return active;
        }
        //Metode til at tilføje en registrering manuelt hvis brugeren vil have det.
        public TEntity CreateRegistration<TBuilder, TEntity>(RegistrationBuilder<TBuilder, TEntity> builder) where TBuilder : RegistrationBuilder<TBuilder, TEntity> where TEntity : Registration
        {
            Guard.AgainstNull(builder, nameof(builder));
            var registration = builder.WithWorkLog(this).Build();
            if (registration is HourRegistration newHourReg)
            {
                AdjustForOverlap(newHourReg);
            }
            if (registration.WorkLogId != this.Id) throw new ArgumentException("Denne registrering tilhører ikke denne log");
            registration.ValidateAgainst(_registrations);
            _registrations.Add(registration);
            UpdatedAt = DateTime.UtcNow;
            return registration;
        }
        public void DeleteRegistration(Guid registrationId)
        {
            var registration = _registrations.FirstOrDefault(r => r.Id == registrationId);
            if (registration == null)
                throw new InvalidOperationException("Registrering ikke fundet.");

            if (ActiveRegistrationId == registrationId)
            {
                ActiveRegistrationId = null;
            }

            registration.SoftDelete();
            UpdatedAt = DateTime.UtcNow;
        }
        //hjælpe metode til når en medarbejder vil tilføje en tidsregistrering der overlapper med andre (hvis de tilføjer en manuelt)
        private void AdjustForOverlap(HourRegistration newReg)
        {
            //find alle overlappende registreringer i workloggen
            var overlaps = _registrations.OfType<HourRegistration>()
                .Where(r => r.StartTime < newReg.EndTime && r.EndTime > newReg.StartTime);

            foreach (var existing in overlaps)
            {
                //hvis den nye registrering fuldstænding dækker den gamle
                if (newReg.StartTime <= existing.StartTime && newReg.EndTime >= existing.EndTime)
                {
                    existing.SoftDelete();
                    continue;
                }
                //hvis den nye ædder ind i starten på en eksisterende en.
                if (newReg.EndTime > existing.StartTime && newReg.EndTime < existing.EndTime)
                {
                    existing.UpdateTimeRange(newReg.EndTime.Value, existing.EndTime.Value);
                }
                //hvis den nye ædder ind i enden på en eksisterende en.
                else if (newReg.StartTime > existing.StartTime && newReg.StartTime < existing.EndTime)
                {
                    existing.UpdateTimeRange(existing.StartTime, newReg.StartTime);
                }
            }
        }
        public WorkLog() { }
    }
}
