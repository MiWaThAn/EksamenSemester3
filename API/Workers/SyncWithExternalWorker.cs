using Application.Interfaces;
using Application.Interfaces.Services;
using Application.Interfaces.Services.Sync;
using Domain.Entity.Mapping;
using Domain.Entity.Person;
using Domain.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace API.Workers
{
    public class SyncWithExternalWorker : BackgroundService
    {
        private readonly ILogger<SyncWithExternalWorker> _logger;
        private const int _intervalBetweenSyncs = 900;
        private readonly IServiceScopeFactory _scopeFactory;
        public SyncWithExternalWorker(ILogger<SyncWithExternalWorker> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

           
            _logger.LogInformation("SyncWithExternalWorker started at: {time}", DateTime.UtcNow);
            while (!stoppingToken.IsCancellationRequested)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    break;
                }

                try
                {
                    _logger.LogInformation("SyncWithExternalWorker is running at: {time}", DateTime.UtcNow);
                   
                    //Tilføjer scopes for at det ikke er de samme instanser der bruges konstant.
                    using var scope = _scopeFactory.CreateScope();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var syncService = scope.ServiceProvider.GetRequiredService<ISyncService>();


                    var companies = await unitOfWork.Companies.GetAllWithIntegrationSettingsAsync();
                    

                    foreach (var company in companies)
                    {
                        await syncService.SyncAllAsync(company);
                    }



                }
                catch (Exception ex)
                { _logger.LogError(ex, "Syncing all data failed"); }
                await Task.Delay(TimeSpan.FromSeconds(_intervalBetweenSyncs), stoppingToken);

            }

        }
    }
}
