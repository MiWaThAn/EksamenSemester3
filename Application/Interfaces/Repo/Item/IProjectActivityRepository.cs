using Domain.Entity.Item.Activity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Repo.Item
{
    public interface IProjectActivityRepository : IGenericRepository<ProjectActivity>
    {
        Task<ProjectActivity?> GetByProjectActivityNumberAsync(string projectActivityNumber);
        Task<IEnumerable<ProjectActivity>> GetByEmployeeIdAsync(Guid employeeId);
        Task<IEnumerable<ProjectActivity>> GetByProjectIdAsync(Guid projectId);
    }
}
