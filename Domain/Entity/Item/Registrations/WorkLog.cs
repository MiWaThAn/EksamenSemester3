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
        public DateTime DateCreated { get; private set; }
        public Guid? ActiveRegistrationId { get; private set; }

        private readonly List<Registration> _registrations = new();
        public IReadOnlyCollection<Registration> Registrations => _registrations.Where(r => !r.IsDeleted).ToList().AsReadOnly();

        internal WorkLog(Employee employee) : base()
        {
            Guard.AgainstNull(employee, nameof(employee));

            EmployeeId = employee.Id;
            DateCreated = DateTime.UtcNow;
        }

        //Business Methods (UI) 
        //Method for when the user wants to start work on an activity
        public void StartWork(ProjectActivity activity, string? description)
        {
            Guard.AgainstNull(activity, nameof(activity));
            if (ActiveRegistrationId != null && ActiveRegistrationId != Guid.Empty)
                throw new InvalidOperationException("Der er allerede en aktiv registrering.");
            var safeDescription = description ?? string.Empty;
            var builder = new HourRegistrationBuilder()
            .WithProjectActivity(activity)
            .WithDescription(safeDescription)
            .WithStart(DateTime.UtcNow)
            .WithType(TimeType.Work)
            .WithWorkLog(this);

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
            .WithStart(DateTime.UtcNow)
            .WithWorkLog(this);

            ActiveRegistrationId = CreateRegistration(builder).Id;
            UpdatedAt = DateTime.UtcNow;
        }
        //metode til når en bruger fil forsætte arbejde
        public void ResumeWork(ProjectActivity activity, string? description)
        {
            var activeBreak = GetActiveHourRegistration();
            if (activeBreak.ProjectActivityId != null && activeBreak.Type != TimeType.Break)
                throw new InvalidOperationException("Du er ikke på pause.");
            activeBreak.SetEndTime(DateTime.UtcNow);
            var safeDescription = description ?? string.Empty;
            var builder = new HourRegistrationBuilder()
                .WithProjectActivity(activity)
                .WithDescription(safeDescription)
                .WithStart(DateTime.UtcNow)
                .WithType(TimeType.Work)
                .WithWorkLog(this);

            ActiveRegistrationId = CreateRegistration(builder).Id;
            UpdatedAt = DateTime.UtcNow;
        }
        //Metode til når en medarbejder vil skifte opgave
        public void SwitchActivity(ProjectActivity newActivity, string? newDescription)
        {
            Guard.AgainstNull(newActivity, nameof(newActivity));
            var active = GetActiveHourRegistration();
            active.SetEndTime(DateTime.UtcNow);

            var builder = new HourRegistrationBuilder()
                .WithProject(active.ProjectId)
                .WithProjectActivity(newActivity)
                .WithDescription(newDescription ?? string.Empty)
                .WithStart(DateTime.UtcNow)
                .WithType(TimeType.Work)
                .WithWorkLog(this);

            var reg = CreateRegistration(builder);
            ActiveRegistrationId = reg.Id;
        }
        public void SwitchProjectAndActivity(Project newProject, ProjectActivity newActivity, string? newDescription)
        {
            Guard.AgainstNull(newProject, nameof(newProject));
            Guard.AgainstNull(newActivity, nameof(newActivity));

            var active = GetActiveHourRegistration();
            active.SetEndTime(DateTime.UtcNow);

            var builder = new HourRegistrationBuilder()
                .WithProject(newProject) // Her sætter vi det helt nye projekt ind
                .WithProjectActivity(newActivity)
                .WithDescription(newDescription ?? string.Empty)
                .WithStart(DateTime.UtcNow)
                .WithType(TimeType.Work)
                .WithWorkLog(this);

            var reg = CreateRegistration(builder);
            ActiveRegistrationId = reg.Id;
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
            var active = _registrations.OfType<HourRegistration>().FirstOrDefault(r => !r.IsDeleted && !r.IsFinished);
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
            registration.ValidateAgainst(_registrations.Where(r => !r.IsDeleted));
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
            if (newReg.StartTime == null || newReg.EndTime == null) return;

            var overlaps = _registrations.OfType<HourRegistration>()
                .Where(r => !r.IsDeleted && r.IsFinished && r.StartTime < newReg.EndTime && r.EndTime > newReg.StartTime)
                .ToList();

            foreach (var existing in overlaps)
            {
                //ny registrering opsluger den gamle fuldstændigt
                if (newReg.StartTime <= existing.StartTime && newReg.EndTime >= existing.EndTime)
                {
                    existing.SoftDelete();
                    continue;
                }

                //ny registrering ligger inde i en eksisterende
                if (newReg.StartTime > existing.StartTime && newReg.EndTime < existing.EndTime)
                {
                    //gem den gamle slut tid til den nye halvdel
                    var originalEndTime = existing.EndTime.Value;

                    //afkort den eksisterende til at stoppe når den nye starter
                    existing.UpdateTimeRange(existing.StartTime, newReg.StartTime);

                    //opret den resterende del som en ny registrering efter den nye reg slutter
                    var splitReg = new HourRegistrationBuilder()
                        .WithWorkLog(this)
                        .WithProjectActivity(existing.ProjectActivityId)
                        .WithProject(existing.ProjectId)
                        .WithDescription(existing.Description)
                        .WithType(existing.Type)
                        .WithStart(newReg.EndTime.Value)
                        .Build();

                    splitReg.SetEndTime(originalEndTime);
                    _registrations.Add(splitReg);
                    continue;
                }

                //hvis ny æder sig ind i starten af eksisterende
                if (newReg.EndTime > existing.StartTime && newReg.EndTime < existing.EndTime)
                {
                    existing.UpdateTimeRange(newReg.EndTime.Value, existing.EndTime.Value);
                }
                //hvis ny æder sig ind i enden af eksisterende
                else if (newReg.StartTime > existing.StartTime && newReg.StartTime < existing.EndTime)
                {
                    existing.UpdateTimeRange(existing.StartTime, newReg.StartTime);
                }
            }
        }
        public WorkLog() { }
    }
}
