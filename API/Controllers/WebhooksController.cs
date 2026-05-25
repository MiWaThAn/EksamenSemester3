
using Application.Commands.Webhooks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Model.Webhook;

[ApiController]
[Route("api/[controller]")]
public class WebhooksController : ControllerBase
{

    private readonly IMediator _mediator;
    public WebhooksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Receive([FromBody]WebhookPayload payload)
    {
        try
        {
            var command = new HandleWebhookCommand(payload.Cvr,payload.Entity,payload.Url,payload.OldId,payload.Provider);


            await _mediator.Send(command);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message + " || " + ex.InnerException?.Message);
        }
    }

}