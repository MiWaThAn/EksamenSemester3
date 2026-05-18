using Application.DTO;
using Application.Interfaces.Services.Sync;
using Domain.Entity.Mapping.ValueObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Commands.Person.Handlers
{
    public class CustomerSyncHandler : IEntitySyncHandler
    {
        public IntegrationEntityType TargetType => IntegrationEntityType.From("Customer");

        private readonly IEconomicApiClient _apiClient;

        public CustomerSyncHandler(IEconomicApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task ProcessAndSaveAsync(List<BaseIntegrationDTO> dtos, Guid companyId)
        {
            foreach (var dto in dtos)
            {
                var customerJson = await _apiClient.GetCustomerAsync(dto.ExternalId);
            }

        }
    }
}