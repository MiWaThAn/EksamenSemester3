using MediatR;
using Shared.Model.IntegrationSettings;

public record CreateIntegrationSettingCommand(
    Guid AccountId,
    string ProviderName,
    string KeyName,
    string KeyValue,
    List<string> SelectedEntityTypes) : IRequest<CreateIntegrationSettingResponse>;