using MediatR;
using Shared.Item.Registrations.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Item.Registrations.Commands
{
    public record UpdateRegistrationProjectCommand(Guid AccountId,Guid WorkLogId,Guid RegistrationId,Guid NewProjectId,Guid NewProjectActivityId) : IRequest<BaseRegistrationResponse>;
}
