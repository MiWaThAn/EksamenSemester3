using Application.Interfaces.Repo.Item.IRegistrationRepo;
using Domain.Entity.Item.Registrations;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories.Item.Registrations
{
    internal class ExpenseRegistrationRepository : RegistrationRepository<ExpenseRegistration>, IExpenseRegistrationRepository
    {
        public ExpenseRegistrationRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<ExpenseRegistration?> GetByRegistrationNumberAsync(string registrationNumber)
        {
            return await _context.ExpenseRegistrations.FirstOrDefaultAsync(x => x.RegistrationNumber == registrationNumber);
        }
        get by expense id implement infrastructure injecter and error handler pipeline
    }
}
