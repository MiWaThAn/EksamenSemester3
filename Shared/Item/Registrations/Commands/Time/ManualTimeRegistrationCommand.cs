using MediatR;
using Shared.Item.Registrations.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Item.Registrations.Commands.Time
{
    public record ManualTimeRegistrationCommand(Guid AccountId, Guid ProjectId, Guid? ProjectActivityId, DateTime StartTime, DateTime EndTime,string Description, DateTime Date,bool isWork) : IRequest<BaseRegistrationResponse>;
}
