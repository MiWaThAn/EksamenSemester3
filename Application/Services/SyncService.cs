using Application.Interfaces;
using Application.Interfaces.Data;
using Application.Interfaces.Services.Sync;
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

        public SyncService(IExternalAPIService externalAPIService, ILogger<SyncService> logger)
        {
            _externalAPIService = externalAPIService;
            _logger = logger;
        }


        public async Task SyncAllAsync(Company company) 
        {
            var settings = company.Settings;

           
            foreach (var setting in settings)
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


                    await SyncSingleAsync(providerUrl, setting, entityType);
                        
                    
                    
                }
            //Burde jeg give EntityType og providername med videre, så adapter selv kan finde ud af hvor det skal sendes hen til?
            }
        }
        public async Task SyncSingleAsync(ProviderEndpoint endpoint, IntegrationSetting setting, SelectedEntityType entityType)
        {
            string url = endpoint.Url;
            if (url == null)
            {
                _logger.LogWarning("No URL found for entity type: {entityType}", entityType.EntityType);
                return;
            }
            var json =  await _externalAPIService.FetchFromAPI(url, setting.Key, setting.EncryptedValue);

            //Brug data fra ExternalService i infrastructure og adapter hvis der er data som vi ikke har.

            //Gem de data som skal i DB.


        }
    }
}
