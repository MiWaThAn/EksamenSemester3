using Domain.Entity.Item.Registrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Repo.Item.IRegistrationRepo
{
    public interface IExpenseRegistrationRepository : IRegistrationRepository<ExpenseRegistration>
    {
        Task<ExpenseRegistration?> GetByRegistrationNumberAsync(string registrationNumber);
    }
}
