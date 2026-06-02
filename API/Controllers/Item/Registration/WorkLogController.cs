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
using Shared.Requests;
using System.Security.Claims;

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
        [HttpGet("active")]
        [ProducesResponseType(typeof(WorkLogDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetActiveWorkLog(CancellationToken ct)
        {
            var employeeId = User.GetEmployeeId();

            var query = new GetActiveWorkLogQuery(employeeId);

            var response = await _mediator.Send(query, ct);

            return response != null
                ? Ok(response)
                : NotFound();
        }

        [HttpGet("history")]
        [ProducesResponseType(typeof(IEnumerable<WorkLogDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetWorkLogHistory(CancellationToken ct)
        {
            var employeeId = User.GetEmployeeId();
            var query = new GetWorkLogHistoryQuery(employeeId);
            var response = await _mediator.Send(query, ct);
            return response != null
                ? Ok(response)
                : NotFound();
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
        [HttpGet("pending-approval")]
        [ProducesResponseType(typeof(IEnumerable<WorkLogDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPendingWorkLogs(CancellationToken ct)
        {
            var companyId = User.GetCompanyId();
            var query = new GetPendingWorkLogsQuery(companyId);
            var response = await _mediator.Send(query, ct);
            return Ok(response);
        }

        //WORKFLOW COMMANDS (POST)
        [HttpPost("start-work")]
        public async Task<IActionResult> StartWork(StartWorkRequest request, CancellationToken ct)
        {
            var employeeId = User.GetEmployeeId();
            var command = new StartWorkCommand(employeeId, request.ProjectId, request.ProjectActivityId);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }
        [HttpPost("take-break")]
        public async Task<IActionResult> TakeBreak(CancellationToken ct)
        {
            var employeeId = User.GetEmployeeId();
            var command = new TakeBreakCommand(employeeId);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpPost("resume-work")]
        public async Task<IActionResult> ResumeWork(CancellationToken ct)
        {
            var employeeId = User.GetEmployeeId();
            var command = new ResumeWorkCommand(employeeId);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpPost("switch-activity/{projectId:guid}/{projectActivityId:guid}")]
        public async Task<IActionResult> SwitchActivity(Guid projectId, Guid projectActivityId, CancellationToken ct)
        {
            var employeeId = User.GetEmployeeId();
            var command = new SwitchActivityCommand(employeeId, projectId, projectActivityId);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpPost("switch-project/{workLogId:guid}/{projectId:guid}/{projectActivityId:guid}")]
        public async Task<IActionResult> SwitchProject(Guid workLogId, Guid projectId, Guid projectActivityId, CancellationToken ct)
        {
            var employeeId = User.GetEmployeeId();
            var command = new SwitchProjectCommand(employeeId, workLogId, projectId, projectActivityId);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpPost("end-work")]
        public async Task<IActionResult> EndWork(CancellationToken ct)
        {
            var employeeId = User.GetEmployeeId();
            var command = new EndWorkCommand(employeeId);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpPost("clock-out")]
        public async Task<IActionResult> ClockOut(CancellationToken ct)
        {
            var employeeId = User.GetEmployeeId();
            var command = new ClockOutCommand(employeeId);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpPost("worklog/{workLogId:guid}/submit")]
        public async Task<IActionResult> SubmitWorkLogForApproval(Guid workLogId, CancellationToken ct)
        {
            var employeeId = User.GetEmployeeId();
            var command = new SubmitWorkLogCommand(workLogId, employeeId);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }
        [Authorize(Roles = "Company")]
        [HttpPost("worklog/{workLogId:guid}/approve")]
        public async Task<IActionResult> ApproveWorkLog(Guid workLogId, CancellationToken ct)
        {
            var companyId = User.GetCompanyId();
            var command = new ApproveWorkLogCommand(workLogId, companyId);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }
        [Authorize(Roles = "Company")]
        [HttpPost("worklog/{workLogId:guid}/reject")]
        public async Task<IActionResult> RejectWorkLog(Guid workLogId, [FromBody] RejectWorkLogRequest request, CancellationToken ct)
        {
            var companyId = User.GetCompanyId();
            var command = new RejectWorkLogCommand(workLogId, companyId, request.Reason);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }


        //REGISTRATIONS MANAGEMENT (POST/PUT/DELETE)
        [HttpPost("worklog/{workLogId:guid}/time-registration")]
        public async Task<IActionResult> CreateTimeRegistration([FromBody] ManualTimeRegistrationCommand command, CancellationToken ct)
        {
            var accountId = User.GetEmployeeId();
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpPost("worklog/{workLogId:guid}/expense-registration")]
        public async Task<IActionResult> CreateExpenseRegistration([FromBody] CreateExpenseCommand command, CancellationToken ct)
        {
            var accountId = User.GetAccountId();
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpDelete("worklog/{workLogId:guid}/time-registration/{registrationId:guid}")]
        public async Task<IActionResult> RemoveTimeRegistration(Guid workLogId, Guid registrationId, CancellationToken ct)
        {
            var accountId = User.GetAccountId();
            var command = new DeleteTimeRegistrationCommand(registrationId, accountId);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpDelete("worklog/{workLogId:guid}/expense-registration/{registrationId:guid}")]
        public async Task<IActionResult> RemoveExpenseRegistration(Guid workLogId, Guid registrationId, CancellationToken ct)
        {
            var accountId = User.GetAccountId();
            var command = new DeleteExpenseRegistrationCommand(accountId, registrationId);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpPut("worklog/{workLogId:guid}/registration/{registrationId:guid}/description")]
        public async Task<IActionResult> UpdateRegistrationDescription(Guid workLogId, Guid registrationId, [FromBody] UpdateDescriptionRequest request, CancellationToken ct)
        {
            var accountId = User.GetAccountId();
            var command = new UpdateRegistrationDescriptionCommand(accountId, registrationId, request.Description);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpPut("worklog/{workLogId:guid}/registration/{registrationId:guid}/interval/{timeIntervalId:guid}")]
        public async Task<IActionResult> UpdateTimeRegistrationInterval(Guid workLogId, Guid registrationId, Guid timeIntervalId, [FromBody] UpdateTimeIntervalRequest request, CancellationToken ct)
        {
            var accountId = User.GetAccountId();
            var command = new UpdateTimeRegistrationIntervalCommand(accountId, workLogId, timeIntervalId, registrationId, request.IsBreak, request.Start, request.End);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }
        [HttpPut("worklog/{workLogId:guid}/registration/{registrationId:guid}/project")]
        public async Task<IActionResult> UpdateRegistrationProject(Guid workLogId, Guid registrationId, [FromBody] UpdateRegistrationProjectRequest request, CancellationToken ct)
        {
            var accountId = User.GetAccountId();
            var command = new UpdateRegistrationProjectCommand(accountId, workLogId, registrationId, request.NewProjectId, request.NewProjectActivityId);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpPut("worklog/{workLogId:guid}/registration/{registrationId:guid}/activity")]
        public async Task<IActionResult> UpdateRegistrationActivity(Guid workLogId, Guid registrationId, [FromBody] UpdateRegistrationActivityRequest request, CancellationToken ct)
        {
            var accountId = User.GetAccountId();
            var command = new UpdateRegistrationActivityCommand(accountId, workLogId, registrationId, request.NewProjectActivityId);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpPut("registration/{registrationId:guid}/expense")]
        public async Task<IActionResult> UpdateRegistrationExpense(Guid registrationId, [FromBody] UpdateRegistrationExpenseRequest request, CancellationToken ct)
        {
            var accountId = User.GetAccountId();
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