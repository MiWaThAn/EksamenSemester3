using Application.Interfaces;
using MediatR;
using Shared.Item.Registrations.Commands.Expenses;
using Shared.Item.Registrations.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Item.Registrations.Expense
{
    public class UpdateRegistrationExpenseHandler : IRequestHandler<UpdateRegistrationExpenseCommand, BaseRegistrationResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateRegistrationExpenseHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<BaseRegistrationResponse> Handle(UpdateRegistrationExpenseCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, cancellationToken);
                var account = await _unitOfWork.Accounts.GetByIdAsync(request.AccountId, cancellationToken);
                if (account == null)
                    return BaseRegistrationResponse.Fail("Account not found.");
                if (!account.EmployeeId.HasValue)
                    return BaseRegistrationResponse.Fail("Account is not associated with an employee.");
                var emp = await _unitOfWork.Employees.GetByIdAsync(account.EmployeeId.Value, cancellationToken);
                if (emp == null)
                    return BaseRegistrationResponse.Fail("Employee not found.");
                var reg = await _unitOfWork.ExpenseRegistrations.GetByIdAsync(request.ExpenseRegistrationId, cancellationToken);
                if (reg == null)
                    return BaseRegistrationResponse.Fail("Expense registration not found.");
                if (reg.EmployeeId != emp.Id)
                    return BaseRegistrationResponse.Fail("Expense does not belong to the employee.");
                if (request.NewExpenseId.HasValue)
                {
                    var expense = await _unitOfWork.Expenses.GetByIdAsync(request.NewExpenseId.Value, cancellationToken);
                    if (expense == null)
                        return BaseRegistrationResponse.Fail("Expense not found.");
                    reg.UpdateExpense(expense.Id);
                }
                if (request.Date.HasValue)
                {
                    reg.UpdateDate(request.Date.Value);
                }
                if (request.Amount.HasValue)
                {
                    reg.UpdateAmount(request.Amount.Value);
                }
                reg.UpdateDescription(request.Description);
                await _unitOfWork.CompleteAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return BaseRegistrationResponse.Ok(reg.Id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return BaseRegistrationResponse.Fail(ex.Message);
            }
        }
    }
}
