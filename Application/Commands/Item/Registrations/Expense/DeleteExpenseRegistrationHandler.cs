using Application.Interfaces;
using Domain.Entity.Item;
using Domain.Entity.Item.Registrations;
using MediatR;
using Shared.Item.Registrations.Commands.Expenses;
using Shared.Item.Registrations.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Item.Registrations.Expense
{
    public class DeleteExpenseRegistrationHandler : IRequestHandler<DeleteExpenseRegistrationCommand, BaseRegistrationResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeleteExpenseRegistrationHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<BaseRegistrationResponse> Handle(DeleteExpenseRegistrationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, cancellationToken);
                var account = await _unitOfWork.Accounts.GetByIdAsync(request.AccountId);
                if(account == null)
                    return new BaseRegistrationResponse { Success = false, Message = "Account ikke fundet."};
                var registration = await _unitOfWork.ExpenseRegistrations.GetByIdAsync(request.RegistrationId);
                if (registration == null)
                {
                    return new BaseRegistrationResponse { Success = false, Message = "Expense registration not found." };
                }
                var expense = await _unitOfWork.Expenses.GetByIdAsync(registration.ExpenseId);
                if(expense == null)
                {
                    return new BaseRegistrationResponse { Success = false, Message = "Associated expense not found." };
                }
                if(registration.Status == RegistrationStatus.Godkendt)
                {
                    return new BaseRegistrationResponse { Success = false, Message = "Approved expense registrations cannot be deleted." };
                }
                _unitOfWork.ExpenseRegistrations.DeleteByIdAsync(registration.Id);
                if (expense.Status != ApprovalStatus.Approved || expense.Status != ApprovalStatus.Pending)
                    _unitOfWork.Expenses.DeleteByIdAsync(expense.Id);
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();
                return new BaseRegistrationResponse { Success = true, Message = "Expense registration deleted successfully." };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new BaseRegistrationResponse { Success = false, Message = $"An error occurred while deleting the expense registration: {ex.Message}" };
            }
        }
    }
}
