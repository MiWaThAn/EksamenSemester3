using Application.Commands.Person;
using Application.Commands.Person.Queries;
using Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Model;

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

        // GET: api/employee/company/{companyId}
        // Denne rute henter medarbejdere for et firma
        [HttpGet("company/{companyId}")]
        public async Task<ActionResult<IEnumerable<CompanyEmployeeModel>>> GetByCompany(Guid companyId)
        {
            var result = await _mediator.Send(new GetEmployeesByCompanyQuery(companyId));
            return Ok(result); 
        }

        [HttpGet("company/employee/{id:guid}")]
        public async Task<IActionResult> GetDetailedEmployee(Guid id)
        {
            var query = new GetDetailedEmployeeQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound($"Kunne ikke finde medarbejderen.");
            }

            return Ok(result);
        }

        // GET: api/employee/{id}
        // Denne rute henter en medarbejder baseret på ID
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDTO>> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetEmployeeByIdQuery(id));

            if (result == null)
            {
                return NotFound(new { Message = $"We couldn't find employee with ID: {id}" });
            }

            return Ok(result);
        }

        // POST: api/employee
        // Denne rute opretter en ny medarbejder
        [HttpPost]
        public async Task<ActionResult<EmployeeDTO>> Create([FromBody] CreateEmployeeCommand command)
        {
            EmployeeDTO result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
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
    }
}