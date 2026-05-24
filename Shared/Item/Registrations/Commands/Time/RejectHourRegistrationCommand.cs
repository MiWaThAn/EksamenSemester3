using MediatR;
using Shared.Item.Registrations.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Item.Registrations.Commands.Time
{
    public record RejectHourRegistrationCommand(Guid HourRegistrationId, Guid OwnerId, string Reason) : IRequest<BaseRegistrationResponse>;
}
