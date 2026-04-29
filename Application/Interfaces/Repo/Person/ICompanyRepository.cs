using Domain.Entity.Person;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Repo.Person
{
    public interface ICompanyRepository : IAccountRepository<Company>
    {
        Task<Company?> GetByCVRAsync(string cvrNumber);
        Task<Company?> GetByIdWithDetailsAsync(Guid id);
    }
}
