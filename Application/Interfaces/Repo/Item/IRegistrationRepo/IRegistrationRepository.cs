using Domain.Entity.Item;
using Domain.Entity.Item.Registrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Repo.Item.IRegistrationRepo
{
    public interface IRegistrationRepository<T> : IGenericRepository<T> where T : Registration
    {
        Task<IEnumerable<T>> GetByEmployeeIdAsync(Guid employeeId);
        Task<IEnumerable<T>> GetByProjectIdAsync(Guid projectId);
        Task<IEnumerable<T>> GetByActivityIdAsync(Guid activityId);
        Task<IEnumerable<T>> GetByStatusAsync(Status status);
    }
}
