
using Application.DTO;
using Application.Interfaces;
using Application.Interfaces.Data;
using Application.Interfaces.Registries;
using Application.Interfaces.Services.Sync;
using Domain.Builders.Mapping;
using Domain.Entity.Item;
using Domain.Entity.Mapping;
using Domain.Entity.Mapping.ValueObjects;
using Domain.Entity.Person;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Services
{
    public class SyncService : ISyncService
    {
        private readonly IExternalAPIService _externalAPIService;
        
        private readonly ILogger<SyncService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAdapterRegistry _adapterRegistry;
        private readonly IHandlerRegistry _handlerRegistry;
        public SyncService(IExternalAPIService externalAPIService, ILogger<SyncService> logger, IUnitOfWork unitOfWork, IAdapterRegistry adapterRegistry, IHandlerRegistry handlerRegistry)
        {
            _externalAPIService = externalAPIService;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _adapterRegistry = adapterRegistry;
            _handlerRegistry = handlerRegistry;
        }


        public async Task SyncAllAsync(Company company) 
        {
          

           
            foreach (var setting in company.Settings)
            {
                foreach (var entityType in setting.EntityTypes)
                {
                    var providerUrl = setting.Provider.Urls
            .FirstOrDefault(u => u.EntityType == entityType.EntityType);

                    if (providerUrl == null)
                    {
                        _logger.LogWarning("No URL found for entity type: {entityType}", entityType.EntityType);
                        continue;
                    }


                    await SyncSingleAsync(providerUrl, setting, entityType.EntityType);
                        
                    
                    
                }
            
            }
        }
        public async Task SyncSingleAsync(ProviderEndpoint endpoint, IntegrationSetting setting, IntegrationEntityType entityType)
        {
            string url = endpoint.Url;
            if (url == null)
            {
                _logger.LogWarning("No URL found for entity type: {entityType}", entityType);
                return;
            }
            var json =  await _externalAPIService.FetchFromAPI(url, setting.Credential);
            
            
            
            var adapter = _adapterRegistry.GetAdapter(setting.Provider.Datasource);
            var dtos = adapter.Map(json, entityType, setting.CompanyId);
            foreach (var syncEntity in dtos)
                await ProcessAsync(syncEntity, setting, entityType);
            


        }
        private async Task ProcessAsync(ISyncEntity syncEntity,IntegrationSetting setting,IntegrationEntityType entityType)
        {
            var existing = await _unitOfWork.Mappings
                .GetByExternalId(syncEntity.ExternalId, entityType);
            var handler = _handlerRegistry.GetHandler(entityType);
            if (!existing.Any())
                await handler.CreateAsync(syncEntity, setting, entityType);
            else
                await handler.UpdateAsync(syncEntity, existing.First());
        }

        public async Task SyncByExternalIdAsync(IntegrationSetting setting,IntegrationEntityType entityType,string url)
        {
            var endpoint = setting.Provider.Urls
        .FirstOrDefault(u => u.EntityType == entityType);

            if (endpoint == null)
                return;

            

            var json = await _externalAPIService.FetchFromAPI(url, setting.Credential);

            var adapter = _adapterRegistry
                .GetAdapter(setting.Provider.Datasource);
            var syncEntity = adapter.Map(json, entityType, setting.CompanyId).FirstOrDefault();

            await ProcessAsync(syncEntity, setting,entityType);
        }



    }
}
