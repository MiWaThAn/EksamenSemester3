using MediatR;
using Shared.Item.Registrations.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Item.Registrations.Commands
{
    public record ApproveWorkLogCommand(Guid WorkLogId, Guid OwnerId) : IRequest<BaseRegistrationResponse>;
}
