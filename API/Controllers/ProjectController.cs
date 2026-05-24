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

        [HttpGet("company/{companyId:guid}")]
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

        [HttpGet("employee/projects")]
        public async Task<IActionResult> GetEmployeeProjects()
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return BadRequest("API Fejl: Kunne slet ikke finde NameIdentifier-claimet i din JWT-token. Er du logget ordentligt ind?");
                }

                if (!Guid.TryParse(userIdClaim, out Guid loggedInAccountId))
                {
                    return BadRequest($"API Fejl: Det NameIdentifier-claim der blev fundet ({userIdClaim}) kunne ikke laves om til en Guid.");
                }

                var query = new GetProjectsByEmployeeQuery(loggedInAccountId);
                var result = await _mediator.Send(query);

                return Ok(result);
            }
            catch (Exception ex)
            {
                var innerFejl = ex.InnerException != null ? $" -> Inner: {ex.InnerException.Message}" : "";
                return BadRequest($"API Crash i Handler/Repo: {ex.Message}{innerFejl}");
            }
        }
    }
}