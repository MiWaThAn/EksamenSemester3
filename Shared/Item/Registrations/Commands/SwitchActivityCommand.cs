using MediatR;
using Shared.Item.Registrations.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Item.Registrations.Commands
{
    public record SwitchActivityCommand(Guid EmployeeId, Guid ProjectId, Guid NewProjectActivityId) : IRequest<BaseRegistrationResponse>;
}
