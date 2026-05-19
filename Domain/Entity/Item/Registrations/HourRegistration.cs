using Domain.Guards;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity.Item.Registrations
{
    public class HourRegistration : Registration
    {
        public DateTime StartTime { get; internal set; }
        public DateTime? EndTime { get; internal set; }
        public bool IsFinished => EndTime.HasValue;

        public HourRegistration()
        {

        }
        internal HourRegistration(WorkLog workLog, Guid? activityId, DateTime startTime, string description, RegistrationStatus status) : base(workLog, activityId, description, status)
        {
            StartTime = startTime;
        }
        internal override void ValidateAgainst(IEnumerable<Registration> existingRegistrations)
        {
            if (existingRegistrations.Any(r => r.Id == this.Id))
                throw new ArgumentException("Denne registrering er allerede tilføjet.");
            var otherTimes = existingRegistrations.OfType<HourRegistration>();
            if (otherTimes.Any(r => OverlapsWith(r)))
                throw new ArgumentException("Overlappende tidsregistrering fundet.");
        }
        private bool OverlapsWith(HourRegistration other)
        {
            var thisEnd = this.EndTime ?? DateTime.UtcNow;
            var otherEnd = other.EndTime ?? DateTime.UtcNow;
            return this.StartTime < otherEnd && thisEnd > other.StartTime;
        }
        public void UpdateTimeRange(DateTime newStartTime, DateTime newEndTime)
        {
            Guard.AgainstInvalidTimeRange(newStartTime, newEndTime);
            StartTime = newStartTime;
            EndTime = newEndTime;
            UpdatedAt = DateTime.UtcNow;
        }
        public void SetEndTime(DateTime endTime)
        {
            Guard.AgainstInvalidTimeRange(StartTime, endTime);
            EndTime = endTime;
        }
    }
}
