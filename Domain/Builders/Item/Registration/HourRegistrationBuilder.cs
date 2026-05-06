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
        private DateTime End;
        public HourRegistrationBuilder WithStartAndEnd(DateTime startTime, DateTime endTime)
        {
            Guard.AgainstInvalidTimeRange(startTime, endTime);
            Start = startTime;
            End = endTime;
            return this;
        }
        internal override HourRegistration Build()
        {
            Guard.AgainstEmptyGuid(EmployeeId, nameof(EmployeeId));
            Guard.AgainstEmptyGuid(ProjectId, nameof(ProjectId));
            return new HourRegistration(EmployeeId, ProjectId, ActivityId, Start, End, Description, Status);
        }
    }
}