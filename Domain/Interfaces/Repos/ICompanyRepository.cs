using Domain.Entity.Person;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces.Repos
{
    public interface ICompanyRepository : IGenericRepository<Company>
    {
        Task<Company?> GetByCVRAsync(CvrNumber cvrNumber, CancellationToken cancellationToken = default);
        Task<Company?> GetByEmailAsync(EmailAddress emailAddress, CancellationToken cancellationToken = default);
        Task<Company?> GetWithProjectsAsync(Guid Id, CancellationToken cancellationToken = default);
        Task<Company?> GetWithEmployeesAsync(Guid Id, CancellationToken cancellationToken = default);
        Task<Company?> GetWithActivitiesAsync(Guid Id, CancellationToken cancellationToken = default);
        Task<Company?> GetWithExpensesAsync(Guid Id, CancellationToken cancellationToken = default);
        Task<Company?> GetWithAllDetailsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Company?>> GetAllWithIntegrationSettingsAsync();
    }
}
