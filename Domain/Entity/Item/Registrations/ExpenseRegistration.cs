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
        public DateTime Date { get; internal set; }
        public Decimal Amount { get; internal set; }

        public ExpenseRegistration() : base()
        {

        }
        internal ExpenseRegistration(Guid ProjectId,WorkLog workLog, Guid? activityId, Guid expenseId, string description, RegistrationStatus status) : base(ProjectId, workLog, activityId, description, status)
        {
            Guard.AgainstEmptyGuid(expenseId, nameof(expenseId));
            ExpenseId = expenseId;
            Date = DateTime.UtcNow;
        }
        internal override void ValidateAgainst(IEnumerable<Registration> existingRegistrations)
        {
            var otherExpenses = existingRegistrations.OfType<ExpenseRegistration>();
            if (existingRegistrations.Any(r => r.Id == this.Id)) throw new ArgumentException("Denne registrering er allerede tilføjet til medarbejderen.");
        }
        public void UpdateExpense(Guid newExpenseId)
        {
            Guard.AgainstEmptyGuid(newExpenseId, nameof(newExpenseId));
            ExpenseId = newExpenseId;
            UpdatedAt = DateTime.UtcNow;
            MarkAsPending();
        }
        public void UpdateDate(DateTime date)
        {
            Guard.AgainstInvalidTimeRange(DateTime.MinValue, date);
            Date = date;
            UpdatedAt = DateTime.UtcNow;
            MarkAsPending();
        }
        public void UpdateAmount(decimal amount)
        {
            Guard.AgainstNegativeOrZero(amount, nameof(amount));
            Amount = amount;
            UpdatedAt = DateTime.UtcNow;
            MarkAsPending();
        }
    }
}
/// TODO: Implement logic for amount in builder and constructor
