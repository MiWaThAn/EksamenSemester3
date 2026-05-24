using Domain.Entity.Item;
using Domain.Entity.Item.Registrations;
using Domain.Guards;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Builders.Item.Registration
{
    public class HourRegistrationBuilder : RegistrationBuilder<HourRegistrationBuilder, HourRegistration>
    {
        private DateTime Start;
        private DateTime? End;
        private TimeType TimeType;
        public HourRegistrationBuilder WithStart(DateTime startTime)
        {
            Start = startTime;
            return this;
        }
        public HourRegistrationBuilder WithEnd(DateTime endTime)
        {
            End = endTime;
            return this;
        }
        public HourRegistrationBuilder WithType(TimeType timeType)
        {
            TimeType = timeType;
            return this;
        }
        internal override HourRegistration Build()
        {
            return new HourRegistration(ProjectId,WorkLog,ActivityId, Start,End, Status,Description);
        }
    }
}