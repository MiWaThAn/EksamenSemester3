using Application.Interfaces.Services.Sync;
using Domain.Entity;
using Domain.Entity.Mapping.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO
{
    public class SyncEntity<T> :ISyncEntity
    {

        public string ExternalId { get; init; } = string.Empty;     
        public string ObjectVersion { get; init; } = string.Empty;
        public T Data { get; init; } = default!;
        public IntegrationEntityType? ObjectType { get; init; }
        public Guid CompanyId { get; init; }
    }
}
