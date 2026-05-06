using Domain.Guards;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain.Entity.Item.Registrations
{
    public class ExpenseRegistration : Registration
    {
        public Guid ExpenseId { get; internal set; }
        public Expense Expense { get; internal set; }

        public ExpenseRegistration() : base()
        {

        }
        internal ExpenseRegistration(Guid employeeId, Guid projectId, Guid? activityId, Guid expenseId, string description, RegistrationStatus status) : base(employeeId, projectId, activityId, description, status)
        {
            Guard.AgainstEmptyGuid(expenseId, nameof(expenseId));
            ExpenseId = expenseId;
        }
        internal override void ValidateAgainst(IEnumerable<Registration> existingRegistrations)
        {
            var otherExpenses = existingRegistrations.OfType<ExpenseRegistration>();
            if (existingRegistrations.ToList().Exists(r => r.Id == this.Id)) throw new ArgumentException("Denne registrering er allerede tilføjet til medarbejderen.");
        }
        public void UpdateExpense(Guid newExpenseId)
        {
            Guard.AgainstEmptyGuid(newExpenseId, nameof(newExpenseId));
            ExpenseId = newExpenseId;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
