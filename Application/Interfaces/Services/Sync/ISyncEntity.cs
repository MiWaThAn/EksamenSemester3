using Domain.Entity.Mapping.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services.Sync
{
    public interface ISyncEntity
    {
        string ExternalId { get; }

        string ObjectVersion { get; }
        public IntegrationEntityType? ObjectType { get; }
        public Guid CompanyId { get; }
    }
}
