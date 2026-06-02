using MediatR;
using Shared.Item.Registrations.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Item.Registrations.Commands
{
    public record SwitchProjectCommand(Guid EmployeeId,Guid WorkLogId,Guid NewProjectId, Guid NewProjectActivityId) : IRequest<BaseRegistrationResponse>;
}
