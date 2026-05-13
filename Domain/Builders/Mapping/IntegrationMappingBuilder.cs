using Domain.Entity;
using Domain.Entity.Mapping;
using Domain.Entity.Mapping.ValueObjects;
using Domain.Guards;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Builders.Mapping
{
    public class IntegrationMappingBuilder
    {
        private Guid LocalId;
        private IntegrationEntityType EntityType;
        private string ExternalId;
        private IntegrationSetting Setting;
        
        private string ObjectVersion;

        public IntegrationMappingBuilder WithLocalId(Base @base)
        {
            LocalId = @base.Id;
            return this;
        }
        public IntegrationMappingBuilder WithEntityType(IntegrationEntityType integrationEntityType)
        {
            EntityType = integrationEntityType;
            return this;
        }
        public IntegrationMappingBuilder WithExternalId(string externalId)
        {
            Guard.AgainstNullOrEmpty(externalId, nameof(externalId));
            ExternalId = externalId;
            return this;
        }
        
        internal IntegrationMappingBuilder WithSetting(IntegrationSetting setting)
        {
            Guard.AgainstNull(Setting,nameof(Setting));
            //rules;
            setting= Setting;
            return this;
        }
        public IntegrationMappingBuilder WithObjectVersion(string objectVersion)
        {
            ObjectVersion = objectVersion;
            return this;
        }
        internal IntegrationMapping Build()
        {
            Guard.AgainstNullOrEmpty(ExternalId, nameof(ExternalId));
            return new IntegrationMapping(LocalId, EntityType, ExternalId,Setting,ObjectVersion);
        }
    }
}
