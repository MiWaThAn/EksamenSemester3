using Domain.Entity.Person;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces.Repos
{
    public interface ICompanyRepository : IGenericRepository<Company>
    {
        Task<Company?> GetByCVRAsync(CvrNumber cvrNumber);
        Task<Company?> GetByEmailAsync(EmailAddress emailAddress);
        Task<Company?> GetWithProjectsAsync(Guid Id);
        Task<Company?> GetWithEmployeesAsync(Guid Id);
        Task<Company?> GetWithActivitiesAsync(Guid Id);
        Task<Company?> GetWithExpensesAsync(Guid Id);
        Task<Company?> GetWithAllDetailsAsync(Guid id);
        Task<IEnumerable<Company?>> GetAllWithIntegrationSettingsAsync();
    }
}
