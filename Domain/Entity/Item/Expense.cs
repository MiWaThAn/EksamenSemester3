using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity.Item
{
    public class Expense : Base
    {
        public string ExpenseNumber { get; internal set; }
        public string Name { get; internal set; }
        public Guid CompanyId { get; internal set; }
        public DateTime UpdatedAt { get; internal set; }
        public Expense(string expenseNumber, string name, Guid companyId) : base()
        {
            ExpenseNumber = expenseNumber ?? throw new ArgumentNullException(nameof(expenseNumber));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            CompanyId = companyId;
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateExpenseName(string newName)
        {
            Name = newName ?? throw new ArgumentNullException(nameof(newName));
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateExpenseNumber(string newExpenseNumber)
        {
            ExpenseNumber = newExpenseNumber ?? throw new ArgumentNullException(nameof(newExpenseNumber));
            UpdatedAt = DateTime.UtcNow;
        }

    }
}
