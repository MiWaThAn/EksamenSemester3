using Domain.Entity.Item.Registrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Builders.Item.Registration
{
    public class HourRegistrationBuilder : RegistrationBuilder<HourRegistrationBuilder, HourRegistration>
    {
        private DateTime Start;
        private DateTime End;
        public HourRegistrationBuilder WithStartAndEnd(DateTime startTime, DateTime endTime)
        {
            if(startTime > End) throw new ArgumentException("Start time cannot be after end time.");
            Start = startTime;
            End = endTime;
            return this;
        }
        internal override HourRegistration Build()
        {
            if (EmployeeId == Guid.Empty) throw new InvalidOperationException("Employee must be set before building a registration.");
            if (ProjectId == Guid.Empty) throw new InvalidOperationException("Project must be set before building a registration.");
            return new HourRegistration(
                EmployeeId,
                ProjectId,
                ActivityId == Guid.Empty ? null : (Guid?)ActivityId,
                Start,
                End,
                Description);
        }
    }
}