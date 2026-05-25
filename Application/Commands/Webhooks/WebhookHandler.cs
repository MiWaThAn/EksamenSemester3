using Application.Interfaces;
using Application.Interfaces.Services.Sync;
using Domain.Entity.Mapping.ValueObjects;
using Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.ConstrainedExecution;
using System.Text;

namespace Application.Commands.Webhooks
{
    public class WebhookHandler : IRequestHandler<HandleWebhookCommand>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ISyncService _syncService;

        public WebhookHandler(IUnitOfWork unitOfWork, ISyncService syncService)
        {
            _unitOfWork = unitOfWork;
            _syncService = syncService;
        }

        public async Task Handle(HandleWebhookCommand request, CancellationToken cancellationToken)
        {
            var company = await _unitOfWork.Companies
                .GetByCVRWithSettingsAsync(new CvrNumber(request.Cvr));

            if (company == null)
                throw new Exception($"Company with CVR {request.Cvr} not found.");

            var setting = company.Settings
                .FirstOrDefault(s => s.Provider.Datasource.Value == request.Provider);

            if (setting == null)
                throw new Exception($"No integration setting found for provider '{request.Provider}'.");

            var entityType = IntegrationEntityType.From(request.Entity);

            await _syncService.SyncSingleByUrl(setting,entityType,request.Url,request.OldId.ToString());
        }
    }
}
