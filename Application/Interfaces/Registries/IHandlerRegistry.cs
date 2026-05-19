using Application.Interfaces.Handlers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Registries
{
    public interface IHandlerRegistry
    {
        IEntitySyncHandler GetEntitySyncHandler();
    }
}
