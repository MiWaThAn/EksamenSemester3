using Domain.Entity.Mapping;
using Domain.Interfaces.Repos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Repo.Mapping
{
    public interface IIntegrationSettingsRepository : IGenericRepository<IntegrationSetting>
    {
        Task<IEnumerable<IntegrationSetting>> GetByCompanyId(Guid CompanyId, CancellationToken cancellationToken = default);
    }
}
