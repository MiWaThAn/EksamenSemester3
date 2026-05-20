using Application.Interfaces.Adapters;
using Application.Interfaces.Handlers;
using Application.Interfaces.Registries;
using Domain.Entity.Mapping.ValueObjects;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Application.Registries
{
    public class HandlerRegistry : IHandlerRegistry
    {
        private readonly IEnumerable<IEntitySyncHandler> _handlers;

        public HandlerRegistry(IEnumerable<IEntitySyncHandler> handlers)
        {
            _handlers = handlers;
        }



        public IEntitySyncHandler GetHandler(IntegrationEntityType entityType)
        {
            var handler = _handlers
                .FirstOrDefault(h => h.CanHandle(entityType));

            if (handler == null)
            {
                throw new Exception(
                    $"No handler found for '{entityType.Value}'");
            }

            return handler;
        }
    }

    
}

