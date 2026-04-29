using Domain.Entity.Item;
using Domain.Entity.Item.Activity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Repo.Item
{
    public interface IActivityRepository : IGenericRepository<Activity>
    {
        Task<Activity?> GetByActivityNumberAsync(string activityNumber);
        Task<IEnumerable<Activity>> GetByCompanyIdAsync(Guid companyId);
    }
}
