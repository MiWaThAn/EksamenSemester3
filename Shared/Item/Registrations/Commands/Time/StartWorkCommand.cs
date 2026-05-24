using MediatR;
using Shared.Item.Registrations.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Item.Registrations.Commands.Time
{
    public record StartWorkCommand(Guid employeeId, Guid projectId, Guid projectActivityId) : IRequest<BaseRegistrationResponse>;
}
