using Application.DTO;
using Application.Interfaces.Adapters;
using Application.Interfaces.Services.Sync;
using Application.Services;
using Domain.Entity.Mapping;
using Domain.Entity.Mapping.ValueObjects;
using Infrastructure.Webhooks;
using Infrastructure.Webhooks.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

public class EconomicWebhookParser : IWebhookParser
{
    public string ProviderName => "economic";

    
    private readonly IConfiguration _configuration;

    public EconomicWebhookParser( IConfiguration configuration)
    {
        
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

        //MapEventToEntityType(economicEvent.EventType);

        //TODO Der skal tjekkes op på eventtype fra economic, og skal der evt en handler til at styre at hente fra det externalId
        //der er, samt finde vores company evt via deres external company id, så vi kan opdatere den korrekte entity i vores system.
    }

    private IntegrationEntityType MapEventToEntityType(string eventType)
    {
        return eventType switch
        {
            "CUSTOMER_UPDATED" => IntegrationEntityType.From("customer"),
            "EMPLOYEE_UPDATED" => IntegrationEntityType.From("employee"),
            "PROJECT_UPDATED" => IntegrationEntityType.From("project"),
            _ => throw new Exception("Unsupported webhook event")
        };
    }

}

