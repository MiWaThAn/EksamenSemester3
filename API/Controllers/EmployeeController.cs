using Application.Commands.Person;
using Application.Commands.Person.Queries;
using Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Model;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmployeeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/employee/company/employee/{id}
        // Denne rute henter detaljerede oplysninger om en medarbejder baseret på ID
        [HttpGet("company/employee/{id:guid}")]
        public async Task<IActionResult> GetDetailedEmployee(Guid id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out Guid loggedInAccountId)) return Unauthorized();
            var query = new GetDetailedEmployeeQuery(id, loggedInAccountId);
            var result = await _mediator.Send(query);

            if (result == null) return NotFound();
            return Ok(result);
        }

        // POST: api/employee
        // Denne rute opretter en ny medarbejder
        [HttpPost]
        public async Task<ActionResult<EmployeeDTO>> Create([FromBody] CreateEmployeeCommand command)
        {
            EmployeeDTO result = await _mediator.Send(command);

            // Peg på det sikre detalje-endpoint i stedet
            return CreatedAtAction(nameof(GetDetailedEmployee), new { id = result.Id }, result);
        }


        // PUT: api/employee/company/employee/{id}
        // Opdaterer medarbejderens oplysninger
        [HttpPut("company/employee/{id:guid}")]
        public async Task<IActionResult> UpdateEmployee(Guid id, [FromBody] DetailedEmployeeModel model)
        {
            if (model == null || id != model.Id)
            {
                return BadRequest("Data-mismatch eller manglende body");
            }

            var command = new UpdateEmployeeDetailsCommand(
                model.Id,
                model.FullName,
                model.Email,
                model.MobileNumber
            );

            var isSuccess = await _mediator.Send(command);

            if (!isSuccess)
            {
                return NotFound("Kunne ikke opdatere medarbejderen i databasen.");
            }

            return NoContent();
        }
        // GET: api/employee/employee-company
        // Denne rute henter alle medarbejdere i det firma, som den loggede ind medarbejder er tilknyttet
        [HttpGet("employee-company")]
        public async Task<IActionResult> GetMyCompanyEmployees()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid loggedInAccountId))
            {
                return Unauthorized("Ugyldig eller manglende token.");
            }

            var companyId = await _mediator.Send(new GetCompanyIdByAccountIdQuery(loggedInAccountId));

            if (companyId == Guid.Empty)
            {
                return NotFound("Du er ikke tilknyttet et firma.");
            }

            var query = new GetEmployeesByCompanyQuery(companyId, loggedInAccountId);
            var result = await _mediator.Send(query);

            return Ok(result);
        }
    }
}