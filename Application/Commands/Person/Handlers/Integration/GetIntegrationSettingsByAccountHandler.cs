using Application.Interfaces;
using MediatR;
using Shared.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Person.Handlers.Integration
{
    public class GetIntegrationSettingsByAccountHandler(IUnitOfWork _unitOfWork)
    : IRequestHandler<GetIntegrationSettingsByAccountQuery, IEnumerable<IntegrationSettingModel>>
    {
        public async Task<IEnumerable<IntegrationSettingModel>> Handle(GetIntegrationSettingsByAccountQuery request, CancellationToken ct)
        {
            var company = await _unitOfWork.Companies.GetByAccountIdAsync(request.AccountId)
                ?? throw new InvalidOperationException("Firma ikke fundet.");

            return company.Settings.Select(s => new IntegrationSettingModel
            {
                Id = s.Id,
                ProviderName = s.Provider.Datasource.Value,
                KeyName = s.Credential.Key
            });
        }
    }
}
