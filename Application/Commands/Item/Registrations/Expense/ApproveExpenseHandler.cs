using Application.Interfaces;
using MediatR;
using Shared.Item.Registrations.Commands.Expenses;
using Shared.Item.Registrations.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Item.Registrations.Expense
{
    public class ApproveExpenseHandler : IRequestHandler<ApproveExpenseCommand, BaseRegistrationResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ApproveExpenseHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<BaseRegistrationResponse> Handle(ApproveExpenseCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, cancellationToken);
                var registration = await _unitOfWork.ExpenseRegistrations.GetByIdAsync(request.ExpenseRegistrationId, cancellationToken);
                if (registration == null)
                {
                    return new BaseRegistrationResponse { Success = false, Message = "Registrering ikke fundet." };
                }
                var expense = await _unitOfWork.Expenses.GetByIdAsync(registration.ExpenseId, cancellationToken);
                if (expense == null)
                    return new BaseRegistrationResponse { Success = false, Message = "Udgift ikke fundet." };
                var account = await _unitOfWork.Accounts.GetByIdAsync(request.OwnerId, cancellationToken);
                if (account == null)
                    return new BaseRegistrationResponse { Success = false, Message = "Konto ikke fundet." };
                if (!account.CompanyId.HasValue || account.CompanyId.Value != expense.CompanyId)
                    return new BaseRegistrationResponse { Success = false, Message = "Konto er ikke ejer" };
                var company = await _unitOfWork.Companies.GetByIdAsync(account.CompanyId.Value);
                if(company == null)
                    return new BaseRegistrationResponse { Success = false, Message = "Firma ikke fundet" };
                if (request.MakeCategoryGlobal)
                {
                    expense.Approve(company);
                }
                var companyCanAprove = await _unitOfWork.ExpenseRegistrations.CanCompanyModerateAsync(company.Id, registration.Id, cancellationToken);
                if (!companyCanAprove)
                {
                    return new BaseRegistrationResponse { Success = false, Message = "Firmaet har ikke tilladelse til at godkende denne udgift." };
                }
                await _unitOfWork.CompleteAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return new BaseRegistrationResponse { Success = true, Message = "Udgiften er godkendt." };
            }
            catch (InvalidOperationException ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new BaseRegistrationResponse { Success = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new BaseRegistrationResponse { Success = false, Message = "Der opstod en uventet systemfejl." };
            }
        }
    }
}
