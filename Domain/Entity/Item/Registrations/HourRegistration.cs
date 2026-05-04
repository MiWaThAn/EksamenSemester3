using Domain.Guards;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity.Item.Registrations
{
    public class HourRegistration : Registration
    {
        public DateTime StartTime { get; internal set; }
        public DateTime EndTime { get; internal set; }
        public TimeSpan Duration => EndTime - StartTime;

        public HourRegistration()
        {

        }
        internal HourRegistration(Guid employeeId, Guid projectId, Guid? activityId, DateTime startTime, DateTime endTime, string description, RegistrationStatus status) : base(employeeId, projectId, activityId, description, status)
        {
            Guard.AgainstInvalidTimeRange(startTime, endTime);
            StartTime = startTime;
            EndTime = endTime;
        }
        internal override void ValidateAgainst(IEnumerable<Registration> existingRegistrations)
        {
            var otherTimes = existingRegistrations.OfType<HourRegistration>();
            if (existingRegistrations.ToList().Exists(r => r.Id == this.Id)) throw new ArgumentException("Denne registrering er allerede tilføjet til medarbejderen.");
            if (otherTimes.Any(r => OverlapsWith(r))) throw new ArgumentException("Overlappende tidsregistrering fundet for denne medarbejder.");
        }
        private bool OverlapsWith(HourRegistration other)
        {
            return this.StartTime < other.EndTime && this.EndTime > other.StartTime;
        }
        public void UpdateTimeRange(DateTime newStartTime, DateTime newEndTime)
        {
            Guard.AgainstInvalidTimeRange(newStartTime, newEndTime);
            StartTime = newStartTime;
            EndTime = newEndTime;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
