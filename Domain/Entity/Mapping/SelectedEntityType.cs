using Domain.Entity;
using Domain.Entity.Mapping.ValueObjects;
using Domain.Guards;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity.Mapping
{
    public class SelectedEntityType : Base
    {
        public Guid IntegrationSettingId { get; private set; }
        public IntegrationEntityType EntityType { get; private set; }  

        public SelectedEntityType() { }

        internal SelectedEntityType(IntegrationEntityType entityType)
        {
            Guard.AgainstNull(entityType, nameof(entityType));
            EntityType = entityType;
        }
    }
}
