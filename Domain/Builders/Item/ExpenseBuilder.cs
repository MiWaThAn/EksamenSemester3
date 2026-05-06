using Domain.Entity.Item;
using Domain.Entity.Person;
using Domain.Guards;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Builders.Item
{
    public class ExpenseBuilder
    {
        private string Name;
        private Guid CompanyId;
        public ExpenseBuilder WithName(string name)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Name = name;
            return this;
        }
        internal ExpenseBuilder WithCompany(Company company)
        {
            Guard.AgainstNull(company,nameof(company));
            CompanyId = company.Id;
            return this;
        }
        internal Expense Build()
        {
            Guard.AgainstEmptyGuid(CompanyId, nameof(CompanyId));
            Guard.AgainstNullOrEmpty(Name, nameof(Name));
            return new Expense(Name, CompanyId);
        }
    }
}
