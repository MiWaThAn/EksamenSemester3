using Domain.Entity.Mapping;
using Domain.Entity.Person;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services.Sync
{
    public interface ISyncService
    {




        public Task SyncAllAsync(Company company);
        public Task SyncSingleAsync(ProviderEndpoint endpoint, IntegrationSetting setting, SelectedEntityType entityType);
    }
}
