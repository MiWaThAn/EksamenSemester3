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
        internal HourRegistration(Guid employeeId, Guid projectId, Guid? activityId, DateTime startTime, DateTime endTime, string description,string registrationNumber) : base(employeeId, projectId, activityId, description,registrationNumber)
        {
            if(startTime >= endTime) throw new ArgumentException("Start time must be before end time.");
            if(startTime > DateTime.UtcNow) throw new ArgumentException("Start time cannot be in the future.");
            if(endTime > DateTime.UtcNow) throw new ArgumentException("End time cannot be in the future.");
            StartTime = startTime;
            EndTime = endTime;
        }
        internal override void ValidateAgainst(IEnumerable<Registration> existingRegistrations)
        {
            var otherTimes = existingRegistrations.OfType<HourRegistration>();
            if (existingRegistrations.ToList().Exists(r => r.Id == this.Id)) throw new ArgumentException("This registration is already added to the employee.");
            if(otherTimes.Any(r => OverlapsWith(r))) throw new ArgumentException("Overlappende tidsregistrering fundet for denne medarbejder.");
        }

        private bool OverlapsWith(HourRegistration other)
        {
            return this.StartTime < other.EndTime && this.EndTime > other.StartTime;
        }
    }
}
