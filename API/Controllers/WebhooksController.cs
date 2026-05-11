using Application.Interfaces.Adapters;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class WebhooksController : ControllerBase
{
    private readonly IEnumerable<IWebhookParser> _parsers;

    // Vi injicerer en liste af parsers (en for hver udbyder)
    public WebhooksController(IEnumerable<IWebhookParser> parsers)
    {
        _parsers = parsers;
    }

    // URL'en bliver f.eks.: POST /api/webhooks/economic
    [HttpPost("{providerName}")]
    public async Task<IActionResult> Receive(string providerName)
    {
        // 1. Læs hele body'en som en rå tekststreng (fordi vi ikke kender JSON-strukturen endnu)
        using var reader = new StreamReader(Request.Body);
        var rawBody = await reader.ReadToEndAsync();

        // 2. Find den parser, der kan håndtere denne udbyder
        var parser = _parsers.FirstOrDefault(p => p.ProviderName.Equals(providerName, StringComparison.OrdinalIgnoreCase));

        if (parser == null)
            return NotFound("Provider ikke understøttet");

        // 3. Sikkerhed: Tjek om signaturen fra e-conomic/Dinero er gyldig
        if (!parser.ValidateSignature(Request.Headers, rawBody))
            return Unauthorized("Ugyldig webhook signatur");

        // 4. Send den rå JSON videre til parseren
        await parser.ProcessWebhookAsync(rawBody);

        return Ok(); // Svar hurtigt tilbage til udbyderen, så de ikke tror, vi er nede
    }
}