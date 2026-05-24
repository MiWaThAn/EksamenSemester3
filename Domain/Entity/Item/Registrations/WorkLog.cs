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
        public bool IsClosed { get; private set; } = false;
        public DateTime? DateClosed { get; private set; }
        public ApprovalStatus Status { get; private set; } = ApprovalStatus.Draft;
        public string? RejectionReason { get; private set; }
        public DateTime? ReviewedAt { get; private set; }

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
        public void StartWork(Project project, ProjectActivity activity)
        {
            if (Status == ApprovalStatus.Approved)
                throw new InvalidOperationException("Du kan ikke redigere en godkendt log.");
            Guard.AgainstNull(activity, nameof(activity));
            if (ActiveRegistrationId != null && ActiveRegistrationId != Guid.Empty)
                throw new InvalidOperationException("Der er allerede en aktiv registrering.");
            var builder = new HourRegistrationBuilder()
            .WithProject(project)
            .WithProjectActivity(activity)
            .WithStart(DateTime.UtcNow)
            .WithType(TimeType.Work)
            .WithWorkLog(this);

            ActiveRegistrationId = CreateRegistration(builder).Id;
            UpdatedAt = DateTime.UtcNow;
        }
        //metode til når en bruger vil have en pause
        public void TakeBreak()
        {
            if (Status == ApprovalStatus.Approved)
                throw new InvalidOperationException("Du kan ikke redigere en godkendt log.");
            var active = GetActiveHourRegistration();
            active.TakeBreak();
            UpdatedAt = DateTime.UtcNow;
            MarkAsDraft();
        }
        //metode til når en bruger fil forsætte arbejde
        public void ResumeWork()
        {
            var active = GetActiveHourRegistration();
            active.ResumeWork();
            MarkAsDraft();
        }
        //Metode til når en medarbejder vil skifte opgave
        public void SwitchActivity(ProjectActivity newActivity, string? newDescription)
        {
            if (Status == ApprovalStatus.Approved)
                throw new InvalidOperationException("Du kan ikke redigere en godkendt log.");
            Guard.AgainstNull(newActivity, nameof(newActivity));
            var active = GetActiveHourRegistration();
            active.EndWork();

            var builder = new HourRegistrationBuilder()
                .WithProject(active.ProjectId)
                .WithProjectActivity(newActivity)
                .WithDescription(newDescription ?? string.Empty)
                .WithStart(DateTime.UtcNow)
                .WithType(TimeType.Work)
                .WithWorkLog(this);

            var reg = CreateRegistration(builder);
            ActiveRegistrationId = reg.Id;
            UpdatedAt = DateTime.UtcNow;
            MarkAsDraft();
        }
        public void SwitchProjectAndActivity(Project newProject, ProjectActivity newActivity, string? newDescription)
        {
            if (Status == ApprovalStatus.Approved)
                throw new InvalidOperationException("Du kan ikke redigere en godkendt log.");
            Guard.AgainstNull(newProject, nameof(newProject));
            Guard.AgainstNull(newActivity, nameof(newActivity));

            var active = GetActiveHourRegistration();
            active.EndWork();

            var builder = new HourRegistrationBuilder()
                .WithProject(newProject)
                .WithProjectActivity(newActivity)
                .WithDescription(newDescription ?? string.Empty)
                .WithStart(DateTime.UtcNow)
                .WithType(TimeType.Work)
                .WithWorkLog(this);

            var reg = CreateRegistration(builder);
            ActiveRegistrationId = reg.Id;
            UpdatedAt = DateTime.UtcNow;
        }
        //metode for at stoppe arbejde.
        public void EndWork()
        {
            if(Status == ApprovalStatus.Approved)
                throw new InvalidOperationException("Du kan ikke redigere en godkendt log.");
            var active = GetActiveHourRegistration();
            active.EndWork();
            ActiveRegistrationId = null;
            UpdatedAt = DateTime.UtcNow;
            MarkAsDraft();
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
            if (Status == ApprovalStatus.Approved)
                throw new InvalidOperationException("Du kan ikke redigere en godkendt log.");
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
            MarkAsDraft();
            return registration;
        }
        public void DeleteRegistration(Guid registrationId)
        {
            if (Status == ApprovalStatus.Approved)
                throw new InvalidOperationException("Du kan ikke redigere en godkendt log.");
            var registration = _registrations.FirstOrDefault(r => r.Id == registrationId);
            if (registration == null)
                throw new InvalidOperationException("Registrering ikke fundet.");

            if (ActiveRegistrationId == registrationId)
            {
                ActiveRegistrationId = null;
            }

            registration.SoftDelete();
            MarkAsDraft();
            UpdatedAt = DateTime.UtcNow;
        }
        private void MarkAsDraft()
        {
            if (Status == ApprovalStatus.Rejected)
            {
                Status = ApprovalStatus.Draft;
                UpdatedAt = DateTime.UtcNow;
                foreach (var reg in _registrations.Where(r => !r.IsDeleted))
                {
                    reg.MarkAsPending();
                }
            }
        }
        public void Reject(Company company, string reason)
        {
            Guard.AgainstNull(company, nameof(company));
            if (Status == ApprovalStatus.Pending)
            {
                Status = ApprovalStatus.Rejected;
                RejectionReason = reason;
                ReviewedAt = DateTime.UtcNow;
                UpdatedAt = DateTime.UtcNow;
                foreach(var reg in _registrations.Where(r => !r.IsDeleted))
                {
                    reg.Reject(company);
                }
            }
        }
        public void Approve(Company company)
        {
            Guard.AgainstNull(company, nameof(company));
            if (Status == ApprovalStatus.Pending)
            {
                Status = ApprovalStatus.Approved;
                ReviewedAt = DateTime.UtcNow;
                UpdatedAt = DateTime.UtcNow;
                foreach (var reg in _registrations.Where(r => !r.IsDeleted))
                {
                    reg.Approve(company);
                }
            }
        }
        public void SubmitForApproval(Employee employee)
        {
            Guard.AgainstNull(employee, nameof(employee));
            if (Status != ApprovalStatus.Draft)
                throw new InvalidOperationException("Kun kladder kan sendes til godkendelse.");
            if(employee.Id != EmployeeId)
                throw new InvalidOperationException("Kun den tilhørende medarbejder kan sende denne log til godkendelse.");
            Status = ApprovalStatus.Pending;
            UpdatedAt = DateTime.UtcNow;
            foreach (var reg in _registrations.Where(r => !r.IsDeleted))
            {
                reg.MarkAsPending();
            }
        }
        //hjælpe metode til når en medarbejder vil tilføje en tidsregistrering der overlapper med andre (hvis de tilføjer en manuelt)
        private void AdjustForOverlap(HourRegistration newReg)
        {
            if (newReg.StartTime == DateTime.MinValue || newReg.EndTime == null) return;

            var newStart = newReg.StartTime;
            var newEnd = newReg.EndTime.Value;

            //get finished registrations which overlap with new registration
            var overlaps = _registrations.OfType<HourRegistration>()
                .Where(r => !r.IsDeleted && r.IsFinished && r.StartTime < newEnd && r.EndTime > newStart)
                .ToList();

            foreach (var existing in overlaps)
            {
                //new registration completely eats existing registration
                if (newStart <= existing.StartTime && newEnd >= existing.EndTime)
                {
                    existing.SoftDelete();
                    continue;
                }

                //all other cases of partial overlap is handled by the object itself
                //it trims itself and gives us the intervals that were extracted from it (if any)
                var extractedIntervals = existing.TrimAndExtractAfter(newStart, newEnd);

                //if its left empty after that we delete it (no need for empty registrations)
                if (!existing.Intervals.Any())
                {
                    existing.SoftDelete();
                }

                //hvis der blev udtrukket nogle intervaller betyder det at de kom efter den nye registrering, så vi skal have dem ind i en ny registrering der har samme aktivitet og beskrivelse som den gamle
                if (extractedIntervals.Any())
                {
                    var splitReg = new HourRegistrationBuilder()
                        .WithWorkLog(this)
                        .WithProjectActivity(existing.ProjectActivityId)
                        .WithProject(existing.ProjectId)
                        .WithDescription(existing.Description)
                        .Build();

                    splitReg.AddIntervals(extractedIntervals);
                    _registrations.Add(splitReg);
                }
            }
        }
        //metode til at lukke workloggen aka. sendregistreing 
        public void ClockOut()
        {
            if (IsClosed) throw new InvalidOperationException("Arbejdspasset er allerede lukket.");

            if (ActiveRegistrationId != null)
            {
                EndWork();
            }

            IsClosed = true;
            DateClosed = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
        public WorkLog() { }
    }
}
