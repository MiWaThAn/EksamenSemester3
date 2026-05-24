using Application.Interfaces;
using Application.Interfaces.Services;
using Domain.Builders.Mapping;
using Domain.Entity.Mapping.ValueObjects;
using MediatR;
using Shared.Model.IntegrationSettings;

namespace Application.Commands.Person.Handlers.Integration;

public class CreateIntegrationSettingHandler(IUnitOfWork _unitOfWork, IEncryptionService _encryptionService)
    : IRequestHandler<CreateIntegrationSettingCommand, CreateIntegrationSettingResponse>
{
    public async Task<CreateIntegrationSettingResponse> Handle(CreateIntegrationSettingCommand request, CancellationToken cancellationToken)
    {
        bool active = false;
        try
        {
            await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, cancellationToken);
            active = true;

            var company = await _unitOfWork.Companies.GetByAccountIdAsync(request.AccountId)
    ?? throw new InvalidOperationException($"Firma med konto ID {request.AccountId}");

            var datasource = DataSource.From(request.ProviderName);
            var provider = await _unitOfWork.Providers.FindByDatasourceAsync(datasource)
                ?? throw new InvalidOperationException($"Provider '{request.ProviderName}' blev ikke fundet.");

            var entityTypes = request.SelectedEntityTypes
                .Select(IntegrationEntityType.From)
                .ToList();

            var encryptedValue = await _encryptionService.Encrypt(request.KeyValue);

            var builder = new IntegrationSettingBuilder()
                .WithProvider(provider)
                .WithKey(request.KeyName)
                .WithEncryptedValue(encryptedValue)
                .WithIntegrationEntityTypes(entityTypes);

            var setting = company.CreateIntegrationSetting(builder);

            await _unitOfWork.CompleteAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return new CreateIntegrationSettingResponse { Success = true, Id = setting.Id };
        }
        catch (Exception)
        {
            if (active)
                await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}