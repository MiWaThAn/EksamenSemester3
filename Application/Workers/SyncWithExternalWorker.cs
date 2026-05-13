using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Domain.Interfaces;
using System.Linq.Expressions;
using Application.Interfaces.Services;

//namespace Application.Workers
//{
//    public class SyncWithExternalWorker : BackgroundService
//    {
//        private readonly ILogger<SyncWithExternalWorker> _logger;
//        private const int _intervalBetweenSyncs = 900;
//        private readonly IExternalAPIService _externalAPIService;
//        public SyncWithExternalWorker(ILogger<SyncWithExternalWorker> logger, IExternalAPIService externalAPIService)
//        {
//            _logger = logger;
//            _externalAPIService = externalAPIService;
//        }

//        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            _logger.LogInformation("SyncWithExternalWorker started at: {time}", DateTime.UtcNow);
//            while (!stoppingToken.IsCancellationRequested)
//            {
//                if (stoppingToken.IsCancellationRequested)
//                {
//                    break;
//                }

//                try {     
//                _logger.LogInformation("SyncWithExternalWorker is running at: {time}", DateTime.UtcNow);
                    
//                    //Skal have nogle if statements ift de indstillinger firma har sat.




//                }
//                catch (Exception ex)
//                { _logger.LogError(ex, "Syncing all data failed"); }
//                await Task.Delay(TimeSpan.FromSeconds(_intervalBetweenSyncs), stoppingToken);

//            }
           
//            }
//        }
//    }   
