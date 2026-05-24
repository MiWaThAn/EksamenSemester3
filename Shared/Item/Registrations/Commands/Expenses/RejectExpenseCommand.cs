using MediatR;
using Shared.Item.Registrations.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Item.Registrations.Commands.Expenses
{
    public record RejectExpenseCommand(Guid RegistrationId, Guid OwnerId, string Comment) : IRequest<BaseRegistrationResponse>;
}
