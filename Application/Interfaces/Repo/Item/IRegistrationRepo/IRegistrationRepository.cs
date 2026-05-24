using Domain.Entity.Item;
using Domain.Entity.Item.Registrations;
using System;
using System.Collections.Generic;
using Domain.Interfaces.Repos;
using System.Text;

namespace Application.Interfaces.Repo.Item.IRegistrationRepo
{
    public interface IRegistrationRepository<T> : IGenericRepository<T> where T : Registration
    {
        Task<IEnumerable<T>> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetByActivityIdAsync(Guid activityId, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetByStatusAsync(RegistrationStatus status, CancellationToken cancellationToken = default);
        Task<bool> CanCompanyModerateAsync(Guid companyId, Guid registrationId, CancellationToken cancellationToken = default);
    }
}
