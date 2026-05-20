using Domain.Entity.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Repo.Mapping
{
    public interface IIntegrationSettingsRepository
    {
        Task<IEnumerable<IntegrationSetting>> GetByCompanyId(Guid CompanyId, CancellationToken cancellationToken = default);
    }
}
