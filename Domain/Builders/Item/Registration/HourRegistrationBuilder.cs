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
        public HourRegistrationBuilder WithStart(DateTime startTime)
        {
            Start = startTime;
            return this;
        }
        internal override HourRegistration Build()
        {
            return new HourRegistration(WorkLog,ActivityId, Start, Description, Status);
        }
    }
}