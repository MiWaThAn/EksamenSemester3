using Application.Commands.Person.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid loggedInAccountId))
            {
                return Unauthorized("Kunne ikke identificere bruger.");
            }

            var myCompanyId = await _mediator.Send(new GetCompanyIdByAccountIdQuery(loggedInAccountId));

            if (myCompanyId != companyId)
            {
                return Forbid();
            }

            var result = await _mediator.Send(new GetProjectsByCompanyQuery(companyId));
            return Ok(result);
        }

        [HttpGet("company/project/{id:guid}")]
        public async Task<IActionResult> GetDetailedProject(Guid id)
        {

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out Guid loggedInAccountId))
            {
                return Unauthorized("Kunne ikke identificere bruger.");
            }

            var query = new GetDetailedProjectQuery(id, loggedInAccountId);
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
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized("Du er ikke logget ind.");
                }

                if (!Guid.TryParse(userIdClaim, out Guid loggedInAccountId))
                {
                    return BadRequest("Ugyldigt format på bruger-ID.");
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
        [HttpGet("project-company")]
        public async Task<IActionResult> GetMyCompanyProjects()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdClaim, out Guid loggedInAccountId)) return Unauthorized();

            var result = await _mediator.Send(new GetProjectsByCompanyQuery(loggedInAccountId));
            return Ok(result);
        }
    }
}