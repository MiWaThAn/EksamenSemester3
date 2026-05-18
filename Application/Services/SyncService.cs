using Application.Adapters;
using Application.Interfaces;
using Application.Interfaces.Data;
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
        private readonly AdapterRegistry _adapterRegistry;

        public SyncService(IExternalAPIService externalAPIService, ILogger<SyncService> logger, IUnitOfWork unitOfWork, AdapterRegistry adapterRegistry)
        {
            _externalAPIService = externalAPIService;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _adapterRegistry = adapterRegistry;
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
            //Burde jeg give EntityType og providername med videre, så adapter selv kan finde ud af hvor det skal sendes hen til?
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
            var json =  await _externalAPIService.FetchFromAPI(url, setting.Key, setting.EncryptedValue);
            
            
            
            var adapter = _adapterRegistry.GetAdapter(setting.Provider.Datasource);
            var entities = adapter.Map(json, entityType, setting.CompanyId);
            foreach (var syncEntity in entities)
                await ProcessAsync(syncEntity, setting, entityType);
            


        }
        private async Task ProcessAsync(
            SyncEntity syncEntity,
            IntegrationSetting setting,
            IntegrationEntityType entityType)
        {
            var existing = await _unitOfWork.Mappings
                .GetByExternalId(syncEntity.ExternalId, entityType);

            if (!existing.Any())
                await CreateAsync(syncEntity, setting, entityType);
            else
                await UpdateAsync(syncEntity, existing.First());
        }

        private async Task CreateAsync(
    SyncEntity syncEntity,
    IntegrationSetting setting,
    IntegrationEntityType entityType)
        {
            await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);
            try
            {
                if (syncEntity.Entity is Employee employee)
                    await _unitOfWork.Employees.AddAsync(employee);
                else if (syncEntity.Entity is Project project)
                    await _unitOfWork.Projects.AddAsync(project);
                else if (syncEntity.Entity is Customer customer)
                    await _unitOfWork.Customers.AddAsync(customer);

                setting.CreateMapping(new IntegrationMappingBuilder()
                    .WithLocalId(syncEntity.Entity)
                    .WithEntityType(entityType)
                    .WithExternalId(syncEntity.ExternalId)
                    .WithObjectVersion(syncEntity.ObjectVersion));

                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        private async Task UpdateAsync(SyncEntity syncEntity, IntegrationMapping mapping)
        {
            if (mapping.ObjectVersion == syncEntity.ObjectVersion)
                return;

            await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);
            try
            {
                if (syncEntity.Entity is Employee employee)
                {
                    var local = await _unitOfWork.Employees.GetByIdAsync(mapping.LocalId);
                    if (local == null) return;
                    local.UpdateName(employee.Name);
                    if (employee.Email != null)
                        local.UpdateEmail(employee.Email);
                }
                else if (syncEntity.Entity is Project project)
                {
                    var local = await _unitOfWork.Projects.GetByIdAsync(mapping.LocalId);
                    if (local == null) return;
                    local.UpdateProjectName(project.Name);
                }
                else if (syncEntity.Entity is Customer customer)
                {
                    var local = await _unitOfWork.Customers.GetByIdAsync(mapping.LocalId);
                    if (local == null) return;
                    local.UpdateName(customer.Name);
                }

                mapping.UpdateObjectVersion(syncEntity.ObjectVersion);
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }



    }
}
