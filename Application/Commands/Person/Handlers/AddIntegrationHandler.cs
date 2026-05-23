using Application.Commands.Person.Responses;
using Application.Interfaces;
using Application.Interfaces.Services;
using Domain.Builders.Mapping;
using Domain.Builders.Person;
using Domain.Entity.Mapping.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Person.Handlers
{
    public class AddIntegrationHandler : IRequestHandler<AddIntegrationSettingCommand, AddIntagrationSettingReponse>
    {
        private readonly IEncryptionService _encryption;
        private readonly IUnitOfWork _unitOfWork;
        
        public AddIntegrationHandler(IEncryptionService encryption, IUnitOfWork unitOfWork)
        {
            _encryption = encryption;
            _unitOfWork = unitOfWork;
        }
        public async Task<AddIntagrationSettingReponse> Handle(AddIntegrationSettingCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.Serializable, cancellationToken);
            var company = await _unitOfWork.Companies.GetByIdAsync(request.CompanyId);

            var datasource = DataSource.From(request.Datasource);
                var provider = await _unitOfWork.Providers.FindByDatasourceAsync(datasource);
            if (provider == null)
            {
                return new AddIntagrationSettingReponse { Success = false, Message = "Provider not found." };
            }

            var entityTypes = request.SelectedEntityTypes
        .Select(IntegrationEntityType.From)
        .ToList();

            company.CreateIntegrationSetting(new IntegrationSettingBuilder()
                .WithProvider(provider)
                .WithKey(request.Key)
                .WithEncryptedValue(_encryption.Encrypt(request.Value))
                .WithIntegrationEntityTypes(entityTypes));

            await _unitOfWork.CommitTransactionAsync();

            return new AddIntagrationSettingReponse
            {
                Success = true,
                Message = "Integration setting added successfully.",
                Errors = null
            };
        }
    }
}
