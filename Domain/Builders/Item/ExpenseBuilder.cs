using Domain.Entity.Item;
using Domain.Entity.Person;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Builders.Item
{
    public class ExpenseBuilder
    {
        private string ExpenseNumber;
        private string Name;
        private Guid CompanyId;
        public ExpenseBuilder WithExpenseNumber(string ExpenseNumber)
        {
            this.ExpenseNumber = ExpenseNumber;
            return this;
        }
        public ExpenseBuilder WithName(string Name)
        {
            this.Name = Name;
            return this;
        }
        internal ExpenseBuilder WithCompany(Company company)
        {
            if (company == null) throw new ArgumentNullException(nameof(company));
            CompanyId = company.Id;
            return this;
        }
        internal Expense Build()
        {
            if (string.IsNullOrEmpty(Name)) throw new InvalidOperationException("Name is required to build an expense.");
            if (string.IsNullOrEmpty(ExpenseNumber)) throw new InvalidOperationException("Expense number is required to build an expense.");
            if (CompanyId == Guid.Empty) throw new InvalidOperationException("Company ID is required to build an expense.");
            return new Expense(ExpenseNumber, Name, CompanyId);
        }
    }
}
