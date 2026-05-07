using Domain.Entity;
using Domain.Entity.Mapping;
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
        private DataSource Provider;

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
        public IntegrationMappingBuilder WithDataSource(DataSource dataSource)
        {
            Provider = dataSource;
            return this;
        }
        internal IntegrationMappingBuilder WithSetting(IntegrationSetting setting)
        {
            Guard.AgainstNull(Setting,nameof(Setting));
            //rules;
            setting= Setting;
            return this;
        }
        internal IntegrationMapping Build()
        {
            Guard.AgainstNullOrEmpty(ExternalId, nameof(ExternalId));
            return new IntegrationMapping(LocalId, EntityType, ExternalId, Provider, Setting);
        }
    }
}
