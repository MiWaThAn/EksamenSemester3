using Application.Commands.Account;
using Application.Commands.Person.Queries; 
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/account/company-id/{accountId}
        // Henter virksomhedens ID direkte fra kontoen
        [HttpGet("company-id/{accountId:guid}")]
        public async Task<IActionResult> GetCompanyIdFromAccount(Guid accountId)
        {
            var companyId = await _mediator.Send(new GetCompanyIdByAccountIdQuery(accountId));

            if (companyId == Guid.Empty)
            {
                return NotFound("Kontoen blev fundet, men har intet tilknyttet CompanyId.");
            }

            return Ok(new { CompanyId = companyId });
        }

        // POST: api/account/forgot-password
        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
        {
            await _mediator.Send(command);
            return Ok();
        }

        // POST: api/account/reset-password
        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result)
            {
                return BadRequest("Ugyldig eller udløbet token.");
            }

            return Ok();
        }

    }
}