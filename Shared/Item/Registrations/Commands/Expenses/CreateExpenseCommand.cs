using MediatR;
using Shared.Item.Registrations.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Item.Registrations.Commands.Expenses
{
    public record CreateExpenseCommand(Guid AccountId, decimal Amount, string Description, Guid? ExpenseCategoryId, string? NewCategoryName,Guid? ProjectActivityId, Guid ProjectId) : IRequest<BaseRegistrationResponse>;
}
