using Application.Interfaces.Handlers;
using Domain.Entity.Mapping.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Registries
{
    public interface IHandlerRegistry
    {
        public IEntitySyncHandler GetHandler(IntegrationEntityType entityType);
        IEntitySyncHandler GetEntitySyncHandler();
    }
}
