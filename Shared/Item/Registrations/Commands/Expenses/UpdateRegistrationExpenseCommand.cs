using MediatR;
using Shared.Item.Registrations.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Item.Registrations.Commands.Expenses
{
    public record UpdateRegistrationExpenseCommand(
        Guid AccountId,
        Guid ExpenseRegistrationId,
        Guid? NewExpenseId,
        decimal? Amount,
        string? Description,
        DateTime? Date
    ) : IRequest<BaseRegistrationResponse>;
}
