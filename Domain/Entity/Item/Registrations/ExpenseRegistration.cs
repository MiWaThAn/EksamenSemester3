using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity.Item.Registrations
{
    public class ExpenseRegistration : Registration
    {
        public Guid ExpenseId { get; internal set; }
        internal ExpenseRegistration(Guid employeeId, Guid projectId, Guid? activityId, Guid expenseId, string description, string registrationNumber) : base(employeeId, projectId, activityId, description,registrationNumber)
        {
            ExpenseId = expenseId;
        }
        internal override void ValidateAgainst(IEnumerable<Registration> existingRegistrations)
        {
            var otherTimes = existingRegistrations.OfType<ExpenseRegistration>();
            if (existingRegistrations.ToList().Exists(r => r.Id == this.Id)) throw new ArgumentException("This registration is already added to the employee.");
        }
    }
}
