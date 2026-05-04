using Application.Interfaces.Repo.Item;
using Domain.Entity.Item;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories.Item
{
    internal class ExpenseRepository : GenericRepository<Expense>, IExpenseRepository
    {
        internal ExpenseRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Expense>> GetByCompanyIdAsync(Guid companyId)
        {
            return await _context.Expenses.Where(e => e.CompanyId == companyId).ToListAsync();
        }
    }
}
