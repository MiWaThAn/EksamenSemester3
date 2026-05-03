using Domain.Entity.Person;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces.Repos
{
    public interface ICompanyRepository : IGenericRepository<Company>
    {
        Task<Company?> GetByCVRAsync(string cvrNumber);
        Task<Company?> GetWithProjects(Guid Id);
        Task<Company?> GetWithEmployees(Guid Id);
        Task<Company?> GetWithActivities(Guid Id);
        Task<Company?> GetWithExpenses(Guid Id);
        Task<Company?> GetWithAllDetailsAsync(Guid id);
    }
}
