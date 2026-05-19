using Application.DTO;
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
       
        public Task CreateAsync(SyncEntity syncEntity, IntegrationSetting setting, IntegrationEntityType entityType);
        public Task UpdateAsync(SyncEntity syncEntity, IntegrationMapping mapping);
       
    }
}
