using Application.Commands.Person.Queries; 
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
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
        // Henter virksomhedens ID direkte fra kontoen, legend!
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
    }
}