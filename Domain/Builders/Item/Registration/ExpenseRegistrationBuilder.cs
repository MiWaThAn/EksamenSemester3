using Domain.Entity.Item;
using Domain.Entity.Item.Registrations;
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
            ExpenseId = expense.Id;
            return this;
        }
        internal override ExpenseRegistration Build()
        {
            if (EmployeeId == Guid.Empty) throw new InvalidOperationException("Employee must be set before building a registration.");
            if (ProjectId == Guid.Empty) throw new InvalidOperationException("Project must be set before building a registration.");
            if (ExpenseId == Guid.Empty) throw new InvalidOperationException("Expense must be set before building a registration.");
            return new ExpenseRegistration(EmployeeId, ProjectId, ActivityId, ExpenseId, Description);
        }
    }
}
