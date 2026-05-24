using MediatR;
using Shared.Item.Registrations.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Item.Registrations.Commands
{
    public record UpdateRegistrationActivityCommand(
        Guid AccountId,
        Guid WorkLogId,
        Guid RegistrationId,
        Guid NewProjectActivityId) : IRequest<BaseRegistrationResponse>;
}
