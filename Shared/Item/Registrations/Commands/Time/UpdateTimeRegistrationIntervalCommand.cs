using MediatR;
using Shared.Item.Registrations.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Item.Registrations.Commands.Time
{
    public record UpdateTimeRegistrationIntervalCommand(Guid AccountId,
        Guid RegistrationId,
        Guid TimeIntervalId,
        Guid WorkLogId,
        bool IsBreak,
        DateTime NewStartTime,
        DateTime NewEndTime
    ) : IRequest<BaseRegistrationResponse>;
}
