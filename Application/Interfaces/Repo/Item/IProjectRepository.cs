using Domain.Entity.Item;
using Domain.Entity.Mapping;
using Domain.Interfaces.Repos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Repo.Item
{
    public interface IProjectRepository : IGenericRepository<Project>
    {
        
        Task<IEnumerable<Project>> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Project>> GetByActivityIdAsync(Guid activityId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Project>> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Project>> GetProjectsRelatedToEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default);
        Task<Project?> GetByIdWithDetailsAsync(Guid projectId, CancellationToken cancellationToken = default);
    }
}
