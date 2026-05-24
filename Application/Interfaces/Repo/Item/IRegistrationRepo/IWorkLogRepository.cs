using Domain.Entity.Item.Registrations;
using Domain.Interfaces.Repos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Repo.Item.IRegistrationRepo
{
    public interface IWorkLogRepository : IGenericRepository<WorkLog>
    {
        Task<IEnumerable<WorkLog>> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default);
        Task<WorkLog?> GetTodayByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default);
        Task<WorkLog?> GetByIdWithRegistrationsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<WorkLog?> GetActiveByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default);
    }
}
