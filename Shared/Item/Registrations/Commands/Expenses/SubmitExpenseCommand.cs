using MediatR;
using Shared.Item.Registrations.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Item.Registrations.Commands.Expenses
{
    public record SubmitExpenseCommand(Guid registrationId, Guid EmployeeId) : IRequest<BaseRegistrationResponse>;
}
