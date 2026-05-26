using Application.Commands.Person.Queries;
using Domain.Entity.Item.Registrations;
using Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Item.Registrations.Commands;
using Shared.Item.Registrations.Commands.Expenses;
using Shared.Item.Registrations.Commands.Time;
using Shared.Item.Registrations.DTOs;
using Shared.Item.Registrations.Queries;
using Shared.Person.Auth.Commands;

namespace API.Controllers.Item.Registration
{
    [Authorize(Roles = "Employee")]
    [ApiController]
    [Route("api/[controller]")]
    public class WorkLogController : ControllerBase
    {
        private readonly IMediator _mediator;
        public WorkLogController(IMediator mediator)
        {
            _mediator = mediator;
        }
        //QUERIES (GET)
        [HttpGet("active/{accountId:guid}")]
        [ProducesResponseType(typeof(WorkLogDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetActiveWorkLog(Guid accountId, CancellationToken ct)
        {
            var query = new GetActiveWorkLogQuery(accountId);
            var response = await _mediator.Send(query, ct);
            return response != null ? Ok(response) : NotFound();
        }

        [HttpGet("history/{accountId:guid}")]
        [ProducesResponseType(typeof(IEnumerable<WorkLogDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetWorkLogHistory(Guid accountId, CancellationToken ct)
        {
            var query = new GetWorkLogHistoryQuery(accountId);
            var response = await _mediator.Send(query, ct);
            return Ok(response);
        }

        [HttpGet("{workLogId:guid}")]
        [ProducesResponseType(typeof(WorkLogDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetWorkLogById(Guid workLogId, CancellationToken ct)
        {
            var query = new GetWorkLogByIdQuery(workLogId);
            var response = await _mediator.Send(query, ct);
            return response != null ? Ok(response) : NotFound(new ProblemDetails { Detail = "Work log not found." });
        }

        [Authorize(Roles = "Company")]
        [HttpGet("pending-approval/{accountId:guid}")]
        [ProducesResponseType(typeof(IEnumerable<WorkLogDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPendingWorkLogs(Guid accountId, CancellationToken ct)
        {
            var query = new GetPendingWorkLogsQuery(accountId);
            var response = await _mediator.Send(query, ct);
            return Ok(response);
        }

        //WORKFLOW COMMANDS (POST)

        [HttpPost("{accountId:guid}/start-work/{projectId:guid}/{projectActivityId:guid}")]
        public async Task<IActionResult> StartWork(Guid accountId, Guid projectId, Guid projectActivityId, CancellationToken ct)
        {
            var command = new StartWorkCommand(accountId, projectId, projectActivityId);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }
        [HttpPost("{accountId:guid}/take-break")]
        public async Task<IActionResult> TakeBreak(Guid accountId, CancellationToken ct)
        {
            var command = new TakeBreakCommand(accountId);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpPost("{accountId:guid}/resume-work")]
        public async Task<IActionResult> ResumeWork(Guid accountId, CancellationToken ct)
        {
            var command = new ResumeWorkCommand(accountId);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpPost("{accountId:guid}/switch-activity/{projectId:guid}/{projectActivityId:guid}")]
        public async Task<IActionResult> SwitchActivity(Guid accountId, Guid projectId, Guid projectActivityId, CancellationToken ct)
        {
            var command = new SwitchActivityCommand(accountId, projectId, projectActivityId);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpPost("{accountId:guid}/switch-project/{workLogId:guid}/{projectId:guid}/{projectActivityId:guid}")]
        public async Task<IActionResult> SwitchProject(Guid accountId, Guid workLogId, Guid projectId, Guid projectActivityId, CancellationToken ct)
        {
            var command = new SwitchProjectCommand(accountId, workLogId, projectId, projectActivityId);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpPost("{accountId:guid}/end-work")]
        public async Task<IActionResult> EndWork(Guid accountId, CancellationToken ct)
        {
            var command = new EndWorkCommand(accountId);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpPost("{accountId:guid}/clock-out")]
        public async Task<IActionResult> ClockOut(Guid accountId, CancellationToken ct)
        {
            var command = new ClockOutCommand(accountId);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpPost("{accountId:guid}/worklog/{workLogId:guid}/submit")]
        public async Task<IActionResult> SubmitWorkLogForApproval(Guid workLogId, Guid accountId, CancellationToken ct)
        {
            var command = new SubmitWorkLogCommand(accountId, workLogId);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }
        [Authorize(Roles = "Company")]
        [HttpPost("{accountId:guid}/worklog/{workLogId:guid}/approve")]
        public async Task<IActionResult> ApproveWorkLog(Guid workLogId, Guid accountId, CancellationToken ct)
        {
            var command = new ApproveWorkLogCommand(accountId, workLogId);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }
        [Authorize(Roles = "Company")]
        [HttpPost("{accountId:guid}/worklog/{workLogId:guid}/reject")]
        public async Task<IActionResult> RejectWorkLog(Guid workLogId, Guid accountId, [FromBody] RejectWorkLogRequest request, CancellationToken ct)
        {
            var command = new RejectWorkLogCommand(accountId, workLogId, request.Reason);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }


        //REGISTRATIONS MANAGEMENT (POST/PUT/DELETE)
        [HttpPost("{accountId:guid}/worklog/{workLogId:guid}/time-registration")]
        public async Task<IActionResult> CreateTimeRegistration([FromBody] ManualTimeRegistrationCommand command, CancellationToken ct)
        {
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpPost("{accountId:guid}/worklog/{workLogId:guid}/expense-registration")]
        public async Task<IActionResult> CreateExpenseRegistration([FromBody] CreateExpenseCommand command, CancellationToken ct)
        {
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpDelete("{accountId:guid}/worklog/{workLogId:guid}/time-registration/{registrationId:guid}")]
        public async Task<IActionResult> RemoveTimeRegistration(Guid accountId, Guid workLogId, Guid registrationId, CancellationToken ct)
        {
            var command = new DeleteTimeRegistrationCommand(accountId, registrationId);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpDelete("{accountId:guid}/worklog/{workLogId:guid}/expense-registration/{registrationId:guid}")]
        public async Task<IActionResult> RemoveExpenseRegistration(Guid accountId, Guid workLogId, Guid registrationId, CancellationToken ct)
        {
            var command = new DeleteExpenseRegistrationCommand(accountId, registrationId);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpPut("{accountId:guid}/worklog/{workLogId:guid}/registration/{registrationId:guid}/description")]
        public async Task<IActionResult> UpdateRegistrationDescription(Guid accountId, Guid workLogId, Guid registrationId, [FromBody] UpdateDescriptionRequest request, CancellationToken ct)
        {
            var command = new UpdateRegistrationDescriptionCommand(accountId, registrationId, request.Description);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpPut("{accountId:guid}/worklog/{workLogId:guid}/registration/{registrationId:guid}/interval/{timeIntervalId:guid}")]
        public async Task<IActionResult> UpdateTimeRegistrationInterval(Guid accountId, Guid workLogId, Guid registrationId, Guid timeIntervalId, [FromBody] UpdateTimeIntervalRequest request, CancellationToken ct)
        {
            var command = new UpdateTimeRegistrationIntervalCommand(accountId, workLogId, timeIntervalId, registrationId, request.IsBreak, request.Start, request.End);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }
        [HttpPut("{accountId:guid}/worklog/{workLogId:guid}/registration/{registrationId:guid}/project")]
        public async Task<IActionResult> UpdateRegistrationProject(Guid accountId, Guid workLogId, Guid registrationId, [FromBody] UpdateRegistrationProjectRequest request, CancellationToken ct)
        {
            var command = new UpdateRegistrationProjectCommand(accountId, workLogId, registrationId, request.NewProjectId, request.NewProjectActivityId);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpPut("{accountId:guid}/worklog/{workLogId:guid}/registration/{registrationId:guid}/activity")]
        public async Task<IActionResult> UpdateRegistrationActivity(Guid workLogId, Guid accountId, Guid registrationId, [FromBody] UpdateRegistrationActivityRequest request, CancellationToken ct)
        {
            var command = new UpdateRegistrationActivityCommand(accountId, workLogId, registrationId, request.NewProjectActivityId);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpPut("{accountId:guid}/registration/{registrationId:guid}/expense")]
        public async Task<IActionResult> UpdateRegistrationExpense(Guid accountId, Guid registrationId, [FromBody] UpdateRegistrationExpenseRequest request, CancellationToken ct)
        {
            var command = new UpdateRegistrationExpenseCommand(accountId, registrationId, request.NewExpenseId, request.Amount, request.Description, request.Date);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

    }


    public record RejectWorkLogRequest(string Reason = "");
    public record UpdateDescriptionRequest(string Description = "");
    public record UpdateTimeIntervalRequest(DateTime Start, DateTime End, bool IsBreak);
    public record UpdateRegistrationProjectRequest(Guid NewProjectId, Guid NewProjectActivityId);
    public record UpdateRegistrationActivityRequest(Guid NewProjectActivityId);
    public record UpdateRegistrationExpenseRequest(Guid? NewExpenseId, DateTime? Date, decimal? Amount, string? Description);

}