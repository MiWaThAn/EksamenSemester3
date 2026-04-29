using Domain.Entity.Item;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Repo.Item
{
    public interface IProjectRepository : IGenericRepository<Project>
    {
        Task<Project?> GetByProjectNumberAsync(string projectNumber);
        Task<IEnumerable<Project>> GetByEmployeeIdAsync(Guid employeeId);
        Task<IEnumerable<Project>> GetByActivityIdAsync(Guid activityId);
        Task<IEnumerable<Project>> GetByCompanyIdAsync(Guid companyId);
        Task<IEnumerable<Project>> GetProjectsRelatedToEmployeeAsync(Guid employeeId);
        Task<Project?> GetByIdWithDetailsAsync(Guid projectId);
    }
}
