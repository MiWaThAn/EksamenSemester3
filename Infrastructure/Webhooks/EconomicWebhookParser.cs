using Application.DTO;
using Application.Interfaces.Adapters;
using Domain.Entity.Mapping;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

public class EconomicWebhookParser : IWebhookParser
{
    public string ProviderName => "economic";

    // Her injicerer vi listen af specialister fra vores SyncService design!
    private readonly IEnumerable<IEntitySyncHandler> _handlers;

    public EconomicWebhookParser(IEnumerable<IEntitySyncHandler> handlers)
    {
        _handlers = handlers;
    }

    public bool ValidateSignature(IHeaderDictionary headers, string rawBody)
    {
        // Tjek e-conomics specifikke HMAC signatur i headeren
        var signature = headers["X-Economic-Signature"].ToString();
        return HashUtility.VerifySignature(rawBody, signature, "DIN_HEMMELIGE_WEBHOOK_NØGLE");
    }

    public async Task ProcessWebhookAsync(string rawBody)
    {
        // 1. Konverter e-conomics JSON til deres specifikke objekt
        var economicEvent = JsonSerializer.Deserialize<EconomicWebhookEvent>(rawBody);

        // 2. Map til vores geniale BaseIntegrationDTO
        var dto = new ExpenseDTO
        {
            ExternalId = economicEvent.Data.Id.ToString(),
            ObjectVersion = economicEvent.Data.Version,
            ObjectType = IntegrationEntityType.Expense
            // ... map evt. andre felter
        };

        // 3. Find den rigtige specialist (ExpenseSyncHandler)
        var handler = _handlers.FirstOrDefault(h => h.TargetType == dto.ObjectType);

        // 4. Sæt specialisten i gang med en liste, der kun indeholder vores ene DTO!
        if (handler != null)
        {
            var dtosAsList = new List<BaseIntegrationDTO> { dto };
            await handler.ProcessAndSaveAsync(dtosAsList, economicEvent.CompanyId); // Genbrug af kode!
        }
    }
}
