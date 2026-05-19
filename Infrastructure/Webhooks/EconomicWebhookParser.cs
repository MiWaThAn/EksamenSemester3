using Application.DTO;
using Application.Interfaces.Adapters;
using Application.Interfaces.Services.Sync;
using Domain.Entity.Mapping;
using Domain.Entity.Mapping.ValueObjects;
using Infrastructure.Webhooks;
using Infrastructure.Webhooks.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

public class EconomicWebhookParser : IWebhookParser
{
    public string ProviderName => "economic";

    private readonly IEnumerable<IEntitySyncHandler> _handlers;
    private readonly IConfiguration _configuration;

    public EconomicWebhookParser(IEnumerable<IEntitySyncHandler> handlers, IConfiguration configuration)
    {
        _handlers = handlers;
        _configuration = configuration;
    }

    public bool ValidateSignature(IHeaderDictionary headers, string rawBody)
    {
        var signature = headers["X-Economic-Signature"].ToString();

        var secret = _configuration["EconomicSettings:WebhookSecret"];

        return /*string.Equals(signature, "debug_test") ||*/ HashUtility.VerifySignature(rawBody, signature, secret);
    }

    public async Task ProcessWebhookAsync(string rawBody)
    {
        var economicEvent = JsonSerializer.Deserialize<EconomicWebhookEvent>(rawBody);

        
        var dto = new ExpenseDTO
        {
            ExternalId = economicEvent.Data.Id.ToString(),
            ObjectVersion = economicEvent.Data.Version,
            ObjectType = IntegrationEntityType.From("expense")
        };

        var handler = _handlers.FirstOrDefault(h => h.TargetType == dto.ObjectType);

        if (handler != null)
        {
            var dtosAsList = new List<SyncEntity> { dto };
            await handler.ProcessAndSaveAsync(dtosAsList, economicEvent.CompanyId);
        }
    }
}
