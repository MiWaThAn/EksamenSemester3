using Application.Interfaces.Adapters;
using Application.Interfaces.Services.Sync;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class WebhooksController : ControllerBase
{
    private readonly IEnumerable<IWebhookParser> _parsers;

    public WebhooksController(IEnumerable<IWebhookParser> parsers)
    {
        _parsers = parsers;
    }

    [HttpPost("{providerName}")]
    public async Task<IActionResult> Receive(string providerName)
    {
        try
        {
            using var reader = new StreamReader(Request.Body);
            var rawBody = await reader.ReadToEndAsync();

            var parser = _parsers.FirstOrDefault(p => p.ProviderName.Equals(providerName, StringComparison.OrdinalIgnoreCase));

            if (parser == null)
                return NotFound("Provider ikke understøttet");

            if (!parser.ValidateSignature(Request.Headers, rawBody))
                return Unauthorized("Ugyldig webhook signatur");

            await parser.ProcessWebhookAsync(rawBody);

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message + " || " + ex.InnerException?.Message);
        }
    }
    [HttpGet("test-economic-demo")]
    public async Task<IActionResult> TestDemoAsync([FromServices] IEconomicApiClient apiClient)
    {
        try
        {
            var jsonResult = await apiClient.GetCustomerAsync("");

            return Content(jsonResult, "application/json");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { fejlbesked = ex.Message });
        }
    }
}