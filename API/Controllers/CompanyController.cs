using Application.Commands.Person.Queries;
using Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CompanyController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CompanyDTO>> GetById(Guid id)
        {
            var query = new GetCompanyByIdQuery(id);

            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound(new { Message = $"Company with ID {id} was not found." });
            }

            return Ok(result);
        }

    }
}