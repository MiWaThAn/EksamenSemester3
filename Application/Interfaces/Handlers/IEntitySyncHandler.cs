using Application.DTO;
using Application.Interfaces.Services.Sync;
using Domain.Entity.Mapping;
using Domain.Entity.Mapping.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Handlers
{
    public interface IEntitySyncHandler
    {
        public bool CanHandle(IntegrationEntityType entityType);
       
        public Task CreateAsync(ISyncEntity syncEntity, IntegrationSetting setting, IntegrationEntityType entityType);
        public Task UpdateAsync(ISyncEntity syncEntity, IntegrationMapping mapping);
       
    }
}
