using Domain.Entity.Item.Registrations;
using System;
using System.Collections.Generic;
using Domain.Interfaces.Repos;
using System.Text;

namespace Application.Interfaces.Repo.Item.IRegistrationRepo
{
    public interface IExpenseRegistrationRepository : IRegistrationRepository<ExpenseRegistration>
    {
    }
}
