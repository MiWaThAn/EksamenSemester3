using MediatR;
using Shared.Item.Registrations.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Item.Registrations.Commands.Time
{
    public record TakeBreakCommand(Guid EmployeeId) : IRequest<BaseRegistrationResponse>;
}
