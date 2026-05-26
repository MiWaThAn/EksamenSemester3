using Domain.Entity.Item.Activities;
using System;
using Domain.Interfaces.Repos;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Repo.Item
{
    public interface IProjectActivityRepository : IGenericRepository<ProjectActivity>
    {
        Task<IEnumerable<ProjectActivity>> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProjectActivity>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProjectActivity>> GetByIdsAsNoTrackingAsync(IEnumerable<Guid> ids,  CancellationToken cancellationToken = default);
        Task<ProjectActivity?> GetByIdAsNoTrackingAsync(Guid Id, CancellationToken cancellationToken = default);
    }
}
