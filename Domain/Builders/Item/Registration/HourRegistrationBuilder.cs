using Domain.Entity.Item;
using Domain.Entity.Item.Registrations;
using Domain.Guards;
using System;

namespace Domain.Builders.Item.Registration
{
    public class HourRegistrationBuilder : RegistrationBuilder<HourRegistrationBuilder, HourRegistration>
    {
        private DateTime Start;
        private DateTime? End;
        private bool IsClosed = false;
        private TimeType TimeType = TimeType.Work;

        public HourRegistrationBuilder WithStart(DateTime startTime)
        {
            Start = startTime;
            return this;
        }

        // Fixed: Changed parameter to DateTime? to allow slicing logic and open intervals
        public HourRegistrationBuilder WithEnd(DateTime? endTime)
        {
            End = endTime;
            return this;
        }

        public HourRegistrationBuilder WithType(TimeType timeType)
        {
            TimeType = timeType;
            return this;
        }
        public HourRegistrationBuilder WithStatus(bool isClosed)
        {
            IsClosed = isClosed;
            return this;
        }

        internal override HourRegistration Build()
        {
            return new HourRegistration(ProjectId, WorkLog, ActivityId, Start, End, TimeType, Status, Description,IsClosed);
        }
    }
}