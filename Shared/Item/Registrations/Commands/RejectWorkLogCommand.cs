using MediatR;
using Shared.Item.Registrations.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Item.Registrations.Commands
{
    public record RejectWorkLogCommand(Guid WorkLogId, Guid OwnerId,string reason) : IRequest<BaseRegistrationResponse>;
}
