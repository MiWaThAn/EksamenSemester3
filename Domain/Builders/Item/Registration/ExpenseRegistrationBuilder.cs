using Domain.Entity.Item;
using Domain.Entity.Item.Registrations;
using Domain.Guards;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Builders.Item.Registration
{
    public class ExpenseRegistrationBuilder : RegistrationBuilder<ExpenseRegistrationBuilder, ExpenseRegistration>
    {
        private Guid ExpenseId;
        public ExpenseRegistrationBuilder WithExpense(Expense expense)
        {
            Guard.AgainstNull(expense, nameof(expense));
            ExpenseId = expense.Id;
            return this;
        }
        internal override ExpenseRegistration Build()
        {
            Guard.AgainstEmptyGuid(ExpenseId, nameof(ExpenseId));
            return new ExpenseRegistration(WorkLog,ActivityId, ExpenseId, Description,Status);
        }
    }
}
