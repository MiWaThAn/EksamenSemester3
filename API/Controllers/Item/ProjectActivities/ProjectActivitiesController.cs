using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Item.ProjectActivity;

namespace API.Controllers.Item.ProjectActivities
{
    [ApiController]
    [Route("api/projects")]

    public class ProjectActivitiesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProjectActivitiesController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet("{projectId:guid}/activities/for-project")]
        [ProducesResponseType(typeof(IEnumerable<ProjectActivityDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ProjectActivityDto>>> GetActivitiesForRegistration(Guid projectId)
        {
            if (projectId == Guid.Empty)
            {
                return BadRequest("Kunne ikke finde aktiviteter for projektet");
            }

            var query = new GetProjectActivitiesForProjectQuery(projectId);
            var result = await _mediator.Send(query);

            return Ok(result);
        }
    }
}
