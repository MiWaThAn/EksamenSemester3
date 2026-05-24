using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Item.Registrations.Events.Worklogs
{

    public record WorkLogSubmittedEvent(Guid WorkLogId, Guid OwnerAccountId) : INotification;

    public record WorkLogApprovedEvent(Guid WorkLogId, Guid EmployeeAccountId, string DateString) : INotification;

    public record WorkLogRejectedEvent(Guid WorkLogId, Guid EmployeeAccountId, string Reason) : INotification;
}
