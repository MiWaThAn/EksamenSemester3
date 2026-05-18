namespace Infrastructure.Webhooks.Models;

public class EconomicWebhookEvent
{
    public EconomicData Data { get; set; } = new();
    public Guid CompanyId { get; set; }
}

public class EconomicData
{
    public int Id { get; set; }
    public int Version { get; set; }
}