
using Application.Commands.Person.Handlers;
using Application.Commands.Person.Handlers.Integration;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Model.IntegrationSettings;

namespace API.Controllers
{
    [Authorize(Roles = "Company")]
    [ApiController]
    [Route("api/[controller]")]
    public class IntegrationSettingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public IntegrationSettingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        

        // GET: api/integrationsetting/company/{accountId}
        [HttpGet("company/{accountId:guid}")]
        public async Task<IActionResult> GetByAccount(Guid accountId)
        {
            var result = await _mediator.Send(new GetIntegrationSettingsByAccountQuery(accountId));
            return Ok(result);
        }

        // POST: api/integrationsetting
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateIntegrationSettingCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // DELETE: api/integrationsetting/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, [FromQuery] Guid accountId)
        {
            await _mediator.Send(new DeleteIntegrationSettingCommand(id, accountId));
            return NoContent();
        }

        // GET: api/integrationsetting/providers
        [HttpGet("providers")]
        public async Task<IActionResult> GetProviders()
        {
            var result = await _mediator.Send(new GetAllProvidersQuery());
            return Ok(result);
        }



    }
}