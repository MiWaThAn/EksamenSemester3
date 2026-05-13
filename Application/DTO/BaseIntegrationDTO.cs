using Domain.Entity.Mapping;
using Domain.Entity.Mapping.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO
{
    public class BaseIntegrationDTO
    {
        public string ExternalId { get; set; } = string.Empty;
        public int ObjectVersion { get; set; }
        public IntegrationEntityType ObjectType { get; set; }
    }
}
