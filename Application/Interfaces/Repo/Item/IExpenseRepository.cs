using Domain.Entity.Item;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Interfaces.Repos;

namespace Application.Interfaces.Repo.Item
{
    public interface IExpenseRepository : IGenericRepository<Expense>
    {
        Task<Expense?> GetByExpenseNumberAsync(string expenseNumber);
        Task<IEnumerable<Expense>> GetByCompanyIdAsync(Guid companyId);
    }
}
