
using Application.Commands.Person.Handlers;
using Application.Commands.Person.Handlers.Integration;
using Application.Interfaces.Services.Sync;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Model.IntegrationSettings;
using Application.Interfaces;
using Application.Interfaces.Services.Sync;

namespace API.Controllers
{
    [Authorize(Roles = "Company")]
    [ApiController]
    [Route("api/[controller]")]
    public class IntegrationSettingController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ISyncService _syncService;
        private readonly IUnitOfWork _unitOfWork;
        public IntegrationSettingController(IMediator mediator,ISyncService syncService,IUnitOfWork unitOfWork)
        {
            _mediator = mediator;
            _syncService = syncService;
            _unitOfWork = unitOfWork;
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
        public async Task<IActionResult> Create([FromBody] CreateIntegrationSettingCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);

            if (result != null && result.Success)
            {
                var company = await _unitOfWork.Companies.GetByAccountIdAsync(command.AccountId);
                if (company != null)
                {
                    await _syncService.SyncAllAsync(company);
                }
            }

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

        // POST: api/integrationsetting/{id}/sync?accountId={accountId}
        [HttpPost("{id:guid}/sync")]
        public async Task<IActionResult> Sync(Guid id, [FromQuery] Guid accountId, CancellationToken ct)
        {
            // Load company and trigger a sync for all integration settings for the company
            var company = await _unitOfWork.Companies.GetByAccountIdAsync(accountId);
            if (company == null) return NotFound(new ProblemDetails { Detail = "Company not found." });

            await _syncService.SyncAllAsync(company);
            return Accepted();
        }



    }
}