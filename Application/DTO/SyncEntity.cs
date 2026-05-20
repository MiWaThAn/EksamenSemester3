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

        public string ExternalId { get; set; } = string.Empty;     
        public string ObjectVersion { get; set; } = string.Empty;
        public T Data { get; set; } = default!;
        public IntegrationEntityType? ObjectType { get; set; }
        public Guid CompanyId { get; set; }
    }
}
