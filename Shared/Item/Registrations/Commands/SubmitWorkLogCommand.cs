using MediatR;
using Shared.Item.Registrations.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Item.Registrations.Commands
{
    public record SubmitWorkLogCommand(Guid WorkLogId, Guid AccountId) : IRequest<BaseRegistrationResponse>;
}
