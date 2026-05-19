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
            // Vi sender vores query afsted ind i MediatR-systemet, som automatisk finder vores Handler!
            var result = await _mediator.Send(new GetProjectsByCompanyQuery(companyId));

            return Ok(result);
        }
    }
}