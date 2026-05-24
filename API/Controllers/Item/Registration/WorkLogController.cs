using Application.Commands.Person.Queries;
using Domain.Entity.Item.Registrations;
using Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Item.Registrations.Commands;
using Shared.Item.Registrations.Commands.Expenses;
using Shared.Item.Registrations.Commands.Time;
using Shared.Person.Auth.Commands;

namespace API.Controllers.Item.Registration
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkLogController : ControllerBase
    {
        private readonly IMediator _mediator;
        public WorkLogController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("{AccountId:guid}/{ProjectId:guid}/{ProjectActivityId:guid}/start-work")]
        public async Task<IActionResult> StartWork(Guid AccountId, Guid ProjectId, Guid ProjectActivityId, CancellationToken ct)
        {
            var command = new StartWorkCommand(AccountId, ProjectId, ProjectActivityId);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }
        [HttpPost("{AccountId:guid}/take-break")]
        public async Task<IActionResult> TakeBreak(Guid AccountId, CancellationToken ct)
        {
            var command = new TakeBreakCommand(AccountId);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpPost("{AccountId:guid}/resume-work")]
        public async Task<IActionResult> ResumeWork(Guid AccountId, CancellationToken ct)
        {
            var command = new ResumeWorkCommand(AccountId);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpPost("{AccountId:guid}/{ProjectId:guid}/{ProjectActivityId:guid}/switch-activity")]
        public async Task<IActionResult> SwitchActivity(Guid AccountId, Guid ProjectId, Guid ProjectActivityId, CancellationToken ct)
        {
            var command = new SwitchActivityCommand(AccountId, ProjectId, ProjectActivityId);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpPost("{AccountId:guid}/{WorkLogId:guid}/{ProjectId:guid}/{ProjectActivityId:guid}/switch-project")]
        public async Task<IActionResult> SwitchProject(Guid AccountId, Guid WorkLogId, Guid ProjectId, Guid ProjectActivityId, CancellationToken ct)
        {
            var command = new SwitchProjectCommand(AccountId, WorkLogId, ProjectId, ProjectActivityId);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpPost("{AccountId:guid}/end-work")]
        public async Task<IActionResult> EndWork(Guid AccountId, CancellationToken ct)
        {
            var command = new EndWorkCommand(AccountId);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpPost("{AccountId:guid}/clock-out")]
        public async Task<IActionResult> ClockOut(Guid AccountId, CancellationToken ct)
        {
            var command = new ClockOutCommand(AccountId);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpPost("{AccountId:guid}/{WorkLogId:guid}/create-time-registration")]
        public async Task<IActionResult> CreateTimeRegistration([FromBody] ManualTimeRegistrationCommand command, CancellationToken ct)
        {
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpPost("{AccountId:guid}/{WorkLogId:guid}/create-expense-registration")]
        public async Task<IActionResult> CreateExpenseRegistration([FromBody] CreateExpenseCommand command, CancellationToken ct)
        {
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpPost("{AccountId:guid}/{WorkLogId:guid}/{RegistrationId:guid}/remove-time-registration")]
        public async Task<IActionResult> RemoveTimeRegistration(Guid RegistrationId, Guid AccountId, CancellationToken ct)
        {
            var command = new DeleteTimeRegistrationCommand(AccountId, RegistrationId);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpPost("{AccountId:guid}/{WorkLogId:guid}/{RegistrationId:guid}/remove-expense-registration")]
        public async Task<IActionResult> RemoveExpenseRegistration(Guid RegistrationId, Guid AccountId, CancellationToken ct)
        {
            var command = new DeleteExpenseRegistrationCommand(AccountId, RegistrationId);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpPost("{AccountId:guid}/{WorkLogId:guid}/submit-work-log-for-approval")]
        public async Task<IActionResult> SubmitWorkLogForApproval(Guid WorkLogId, Guid AccountId, CancellationToken ct)
        {
            var command = new SubmitWorkLogCommand(AccountId, WorkLogId);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpPost("{AccountId:guid}/{WorkLogId:guid}/approve-work-log")]
        public async Task<IActionResult> ApproveWorkLog(Guid WorkLogId, Guid AccountId, CancellationToken ct)
        {
            var command = new ApproveWorkLogCommand(AccountId, WorkLogId);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpPost("{AccountId:guid}/{WorkLogId:guid}/reject-work-log")]
        public async Task<IActionResult> RejectWorkLog(Guid WorkLogId, Guid AccountId, [FromBody] RejectWorkLogRequest request, CancellationToken ct)
        {
            var command = new RejectWorkLogCommand(AccountId, WorkLogId, request.Reason);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpPost("{AccountId:guid}/{WorkLogId:guid}/{RegistrationId:guid}/update-registration-description")]
        public async Task<IActionResult> UpdateRegistrationDescription(Guid AccountId, Guid RegistrationId, [FromBody] UpdateDescriptionRequest request, CancellationToken ct)
        {
            var command = new UpdateRegistrationDescriptionCommand(AccountId, RegistrationId, request.Description);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpPost("{AccountId:guid}/{WorkLogId:guid}/{RegistrationId:guid}/{TimeIntervalId:guid}/update-time-registration-interval")]
        public async Task<IActionResult> UpdateTimeRegistrationInterval(Guid AccountId, Guid WorkLogId, Guid TimeIntervalId, Guid RegistrationId, [FromBody] UpdateTimeIntervalRequest request, CancellationToken ct)
        {
            var command = new UpdateTimeRegistrationIntervalCommand(AccountId, WorkLogId, TimeIntervalId, RegistrationId, request.IsBreak, request.Start, request.End);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }
        [HttpPost("{AccountId:guid}/{WorkLogId:guid}/{RegistrationId:guid}/update-registration-project")]
        public async Task<IActionResult> UpdateRegistrationProject(Guid AccountId, Guid WorkLogId, Guid RegistrationId, [FromBody] UpdateRegistrationProjectRequest request, CancellationToken ct)
        {
            var command = new UpdateRegistrationProjectCommand(AccountId, WorkLogId, RegistrationId, request.NewProjectId, request.NewProjectActivityId);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpPost("{AccountId:guid}/{WorkLogId:guid}/{RegistrationId:guid}/update-registration-activity")]
        public async Task<IActionResult> UpdateRegistrationActivity(Guid WorkLogId, Guid AccountId, Guid RegistrationId, [FromBody] UpdateRegistrationActivityRequest request, CancellationToken ct)
        {
            var command = new UpdateRegistrationActivityCommand(AccountId, WorkLogId, RegistrationId, request.NewProjectActivityId);
            var response = await _mediator.Send(command, ct);
            return response.Success ? Ok(new { id = response.Id }) : BadRequest(new ProblemDetails { Detail = response.Message });
        }

        [HttpPost("{AccountId:guid}/{RegistrationId:guid}/update-registration-expense")]
        public async Task<IActionResult> UpdateRegistrationExpense(Guid AccountId, Guid RegistrationId, [FromBody] UpdateRegistrationExpenseRequest request, CancellationToken ct)
        {
            var command = new UpdateRegistrationExpenseCommand(AccountId, RegistrationId, request.NewExpenseId, request.Amount, request.Description, request.Date);
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