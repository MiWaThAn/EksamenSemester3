using Domain.Entity.Item;
using Domain.Entity.Item.Activities;
using Domain.Interfaces.Repos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Repo.Item
{
    public interface IActivityRepository : IGenericRepository<Activity>
    {
        Task<IEnumerable<Activity>> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);
    }
}
