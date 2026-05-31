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
        public Guid EmployeeId { get; internal set; }
        public DateTime DateCreated { get; internal set; }
        public Guid? ActiveRegistrationId { get; internal set; }
        public DateTime LastActivityEndTime => _registrations.OfType<HourRegistration>().Where(r => !r.IsDeleted && r.EndTime != null).OrderByDescending(r => r.EndTime).FirstOrDefault()?.EndTime ?? DateTime.MinValue;
        public bool HasActiveRegistration => ActiveRegistrationId != null;
        public bool IsClosed { get; internal set; } = false;
        public DateTime? DateClosed { get; internal set; }
        public ApprovalStatus Status { get; internal set; } = ApprovalStatus.Draft;
        public string? RejectionReason { get; internal set; }
        public DateTime? ReviewedAt { get; internal set; }

        private readonly List<Registration> _registrations = new();
        public IReadOnlyCollection<Registration> Registrations => _registrations.Where(r => !r.IsDeleted).ToList().AsReadOnly();
        public DateTime LastRemindedAt { get; internal set; } = DateTime.UtcNow;

        internal WorkLog(Employee employee) : base()
        {
            Guard.AgainstNull(employee, nameof(employee));

            EmployeeId = employee.Id;
            DateCreated = DateTime.UtcNow;
        }

        //Business Methods (UI) 
        //Method for when the user wants to start work on an activity
        public HourRegistration StartWork(Project project, ProjectActivity activity, Employee employee)
        {
            if (Status == ApprovalStatus.Approved)
                throw new InvalidOperationException("Du kan ikke redigere en godkendt log.");
            Guard.AgainstNull(activity, nameof(activity));
            if (ActiveRegistrationId != null && ActiveRegistrationId != Guid.Empty)
                throw new InvalidOperationException("Der er allerede en aktiv registrering.");
            if (employee.Id != EmployeeId)
                throw new InvalidOperationException("Kun den tilhørende medarbejder kan starte arbejde på denne log.");

            var builder = new HourRegistrationBuilder()
                .WithProject(project)
                .WithProjectActivity(activity)
                .WithStart(DateTime.UtcNow)
                .WithType(TimeType.Work)
                .WithWorkLog(this);

            var reg = CreateRegistration(builder);
            ActiveRegistrationId = reg.Id;
            UpdatedAt = DateTime.UtcNow;
            return reg;
        }
        //metode til når en bruger vil have en pause
        public void TakeBreak(Employee employee)
        {
            if (Status == ApprovalStatus.Approved)
                throw new InvalidOperationException("Du kan ikke redigere en godkendt log.");
            if (employee.Id != EmployeeId)
                throw new InvalidOperationException("Kun den tilhørende medarbejder kan tage pause på denne log.");

            var active = GetActiveHourRegistration();
            active.TakeBreak();
            UpdatedAt = DateTime.UtcNow;
            MarkAsDraft();
        }
        //metode til når en bruger fil forsætte arbejde
        public void ResumeWork(Employee employee)
        {
            if (Status == ApprovalStatus.Approved || Status == ApprovalStatus.Pending)
                throw new InvalidOperationException("Du kan ikke redigere en godkendt eller pending log.");
            if (employee.Id != EmployeeId)
                throw new InvalidOperationException("Kun den tilhørende medarbejder kan genoptage arbejdet på denne log.");
            if(ActiveRegistrationId != null)
            {
                var active = GetActiveHourRegistration();
                active.ResumeWork();
            }
            MarkAsDraft();
        }
        //Metode til når en medarbejder vil skifte opgave
        public HourRegistration SwitchActivity(ProjectActivity newActivity, Employee employee)
        {
            if (Status == ApprovalStatus.Approved)
                throw new InvalidOperationException("Du kan ikke redigere en godkendt log.");
            Guard.AgainstNull(newActivity, nameof(newActivity));
            if (employee.Id != EmployeeId)
                throw new InvalidOperationException("Kun den tilhørende medarbejder kan skifte aktivitet på denne log.");

            var active = GetActiveHourRegistration();
            if(active.HasActive())
                active.EndWork();

            var builder = new HourRegistrationBuilder()
                .WithProject(active.ProjectId)
                .WithProjectActivity(newActivity)
                .WithStart(DateTime.UtcNow)
                .WithType(TimeType.Work)
                .WithWorkLog(this);

            var reg = CreateRegistration(builder);
            ActiveRegistrationId = reg.Id;
            UpdatedAt = DateTime.UtcNow;
            MarkAsDraft();
            return reg;
        }
        public HourRegistration SwitchProjectAndActivity(Project newProject, ProjectActivity newActivity, Employee employee)
        {
            if (Status == ApprovalStatus.Approved)
                throw new InvalidOperationException("Du kan ikke redigere en godkendt log.");
            Guard.AgainstNull(newProject, nameof(newProject));
            Guard.AgainstNull(newActivity, nameof(newActivity));
            Guard.AgainstNull(employee, nameof(employee));
            if (employee.Id != EmployeeId)
                throw new InvalidOperationException("Kun den tilhørende medarbejder kan skifte projekt og aktivitet på denne log.");

            var active = GetActiveHourRegistration();
            if(active.HasActive())
                active.EndWork();

            var builder = new HourRegistrationBuilder()
                .WithProject(newProject)
                .WithProjectActivity(newActivity)
                .WithStart(DateTime.UtcNow)
                .WithType(TimeType.Work)
                .WithWorkLog(this);

            var reg = CreateRegistration(builder);
            ActiveRegistrationId = reg.Id;
            UpdatedAt = DateTime.UtcNow;
            MarkAsDraft();
            return reg;
        }
        //metode for at stoppe arbejde.
        public void EndWork(Employee employee)
        {
            if (Status == ApprovalStatus.Approved || Status == ApprovalStatus.Pending)
                throw new InvalidOperationException("Du kan ikke redigere en godkendt log.");
            if (employee.Id != EmployeeId)
                throw new InvalidOperationException("Kun den tilhørende medarbejder kan afslutte arbejdet på denne log.");

            if(ActiveRegistrationId != null)
            {
                var active = GetActiveHourRegistration();
                if(active.HasActive())
                    active.EndWork();
            }
            UpdatedAt = DateTime.UtcNow;
            MarkAsDraft();
        }
        public void Remind() => LastRemindedAt = DateTime.UtcNow;
        public HourRegistration GetActiveHourRegistration()
        {
            var active = _registrations.OfType<HourRegistration>().FirstOrDefault(r => !r.IsDeleted && !r.IsFinished && r.Id == ActiveRegistrationId);
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
            if(ActiveRegistrationId != null)
            {
                var active = GetActiveHourRegistration();
                active.EndWork();
            }
            var registration = builder.WithWorkLog(this).Build();
            if (registration.EmployeeId != EmployeeId)
                throw new InvalidOperationException("Registreringen skal tilhøre den samme medarbejder som loggen.");

            if (registration is HourRegistration newHourReg)
            {
                AdjustForOverlap(newHourReg);
            }
            if (registration.WorkLogId != this.Id)
                throw new ArgumentException("Denne registrering tilhører ikke denne log");

            registration.ValidateAgainst(_registrations.Where(r => !r.IsDeleted));

            _registrations.Add(registration);
            UpdatedAt = DateTime.UtcNow;
            MarkAsDraft();
            return registration;
        }
        public double CalculateTotalHoursWorked()
        {
            return _registrations.OfType<HourRegistration>()
                .Where(r => !r.IsDeleted)
                .Sum(r => r.TotalHours());
        }

        public double CalculateHoursSinceLastBreak()
        {
            var regs = _registrations.OfType<HourRegistration>();
            double total = 0;
            foreach (var reg in regs)
            {
                if (reg.HasHadBreak())
                {
                    total = 0;
                }
                else
                {
                    total += reg.HoursSinceBreak();
                }
            }
            return total;
        }
        public void UpdateActiveRegistrationInterval(DateTime? newStart, DateTime? newEnd, Employee employee, Guid registrationId, Guid intervalId, TimeType timeType)
        {
            if (Status == ApprovalStatus.Approved)
                throw new InvalidOperationException("Du kan ikke redigere en godkendt log.");
            if (employee.Id != EmployeeId)
                throw new InvalidOperationException("Kun den tilhørende medarbejder kan redigere denne log.");

            var reg = _registrations.FirstOrDefault(r => r.Id == registrationId && !r.IsDeleted);
            if (reg == null)
                throw new InvalidOperationException("Registrering ikke fundet.");
            if (reg is not HourRegistration hourReg)
                throw new InvalidOperationException("Kun timeregistreringer kan have intervallet redigeret.");
            if (hourReg.IsFinished)
                throw new InvalidOperationException("Du kan kun redigere intervallet på en aktiv registrering.");

            hourReg.UpdateTimeInterval(intervalId, newStart, newEnd, timeType);
            UpdatedAt = DateTime.UtcNow;
            MarkAsDraft();
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
        public void UpdateProjectAndActivity(Project newProject, ProjectActivity newActivity, Employee employee, Guid registrationId)
        {
            if (Status == ApprovalStatus.Approved)
                throw new InvalidOperationException("Du kan ikke redigere en godkendt log.");
            if (employee.Id != EmployeeId)
                throw new InvalidOperationException("Kun den tilhørende medarbejder kan redigere denne log.");
            var reg = _registrations.FirstOrDefault(r => r.Id == registrationId && !r.IsDeleted);
            if (reg == null)
                throw new InvalidOperationException("Registrering ikke fundet.");
            reg.LinkToProjectAndActivity(newProject.Id, newActivity.Id);
            UpdatedAt = DateTime.UtcNow;
            MarkAsDraft();
        }
        public void UpdateActivity(ProjectActivity newActivity, Employee employee, Guid registrationId)
        {
            if (Status == ApprovalStatus.Approved)
                throw new InvalidOperationException("Du kan ikke redigere en godkendt log.");
            if (employee.Id != EmployeeId)
                throw new InvalidOperationException("Kun den tilhørende medarbejder kan redigere denne log.");
            var reg = _registrations.FirstOrDefault(r => r.Id == registrationId && !r.IsDeleted);
            if (reg == null)
                throw new InvalidOperationException("Registrering ikke fundet.");
            reg.LinkToActivity(newActivity.Id);
            UpdatedAt = DateTime.UtcNow;
            MarkAsDraft();
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
                foreach (var reg in _registrations.Where(r => !r.IsDeleted))
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
            if (employee.Id != EmployeeId)
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

            //get registrations which have an end time and overlap with new registration
            //We consider registrations that have an EndTime set (ended via EndWork or ClockOut),
            //not only those marked IsFinished. EndWork sets an end time but leaves IsFinished false,
            //so using EndTime ensures we detect and adjust those as well.
            var overlaps = _registrations.OfType<HourRegistration>()
                            .Where(r => !r.IsDeleted && r.EndTime != null && r.StartTime < newEnd && r.EndTime > newStart)
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
                    var firstInterval = extractedIntervals.First();

                    var splitBuilder = new HourRegistrationBuilder()
                        .WithWorkLog(this)
                        .WithProjectActivity(existing.ProjectActivityId)
                        .WithProject(existing.ProjectId)
                        .WithDescription(existing.Description)
                        .WithStart(firstInterval.StartTime);

                    if (firstInterval.EndTime.HasValue)
                    {
                        splitBuilder.WithEnd(firstInterval.EndTime.Value);
                    }

                    var splitReg = splitBuilder.Build();

                    // If there are any remaining trailing intervals, add them safely
                    if (extractedIntervals.Count > 1)
                    {
                        splitReg.AddIntervals(extractedIntervals.Skip(1));
                    }

                    _registrations.Add(splitReg);
                }
            }
        }
        //metode til at lukke workloggen aka. sendregistreing 
        public void ClockOut(Employee employee)
        {
            if (IsClosed) throw new InvalidOperationException("Arbejdspasset er allerede lukket.");
            if (employee.Id != EmployeeId)
                throw new InvalidOperationException("Kun den tilhørende medarbejder kan lukke denne log.");
            if (ActiveRegistrationId != null)
            {
                HourRegistration active = GetActiveHourRegistration();
                active.ClockOut();
                ActiveRegistrationId = null;
            }
            IsClosed = true;
            DateClosed = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
        public WorkLog() { }
    }
}
