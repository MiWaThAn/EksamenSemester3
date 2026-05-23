using Application.Commands.Person.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static Application.Commands.Person.Queries.GetProjectsByCompanyQuery;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProjectController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("company/{companyId}")]
        public async Task<IActionResult> GetCompanyProjects(Guid companyId)
        {
            var result = await _mediator.Send(new GetProjectsByCompanyQuery(companyId));

            return Ok(result);
        }

        [HttpGet("company/project/{id:guid}")]
        public async Task<IActionResult> GetDetailedProject(Guid id)
        {
            var query = new GetDetailedProjectQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound($"Kunne ikke finde projektet med ID: {id}");
            }

            return Ok(result);
        }
    }
}