using Application.DTO;
using Application.Interfaces.Adapters;
using Application.Interfaces.Services.Sync;
using Domain.Entity.Mapping;
using Domain.Entity.Mapping.ValueObjects;
using Infrastructure.Webhooks;
using Infrastructure.Webhooks.Models;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

public class EconomicWebhookParser : IWebhookParser
{
    public string ProviderName => "economic";

    private readonly IEnumerable<IEntitySyncHandler> _handlers;

    public EconomicWebhookParser(IEnumerable<IEntitySyncHandler> handlers)
    {
        _handlers = handlers;
    }

    public bool ValidateSignature(IHeaderDictionary headers, string rawBody)
    {
        var signature = headers["X-Economic-Signature"].ToString();
        return HashUtility.VerifySignature(rawBody, signature, "DIN_HEMMELIGE_WEBHOOK_NØGLE");
    }

    public async Task ProcessWebhookAsync(string rawBody)
    {
        var economicEvent = JsonSerializer.Deserialize<EconomicWebhookEvent>(rawBody);

        var dto = new ExpenseDTO
        {
            ExternalId = economicEvent.Data.Id.ToString(),
            ObjectVersion = economicEvent.Data.Version,
            //ObjectType = IntegrationEntityType.Expense
        };

        var handler = _handlers.FirstOrDefault(h => h.TargetType == dto.ObjectType);

        if (handler != null)
        {
            var dtosAsList = new List<BaseIntegrationDTO> { dto };
            await handler.ProcessAndSaveAsync(dtosAsList, economicEvent.CompanyId);
        }
    }
}
