using Application.Interfaces;
using MediatR;
using Shared.Item.Registrations.Commands.Expenses;
using Shared.Item.Registrations.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Item.Registrations.Expense
{
    public class RejectExpenseHandler : IRequestHandler<RejectExpenseCommand, BaseRegistrationResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        public RejectExpenseHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<BaseRegistrationResponse> Handle(RejectExpenseCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, cancellationToken);
                var registration = await _unitOfWork.ExpenseRegistrations.GetByIdAsync(request.RegistrationId, cancellationToken);
                if (registration == null)
                    return new BaseRegistrationResponse { Success = false, Message = "Udgiftsregistrering ikke fundet." };
                var expense = await _unitOfWork.Expenses.GetByIdAsync(registration.ExpenseId, cancellationToken);
                if (expense == null)
                    return new BaseRegistrationResponse { Success = false, Message = "Udgiftsregistrering ikke fundet." };
                var company = await _unitOfWork.Companies.GetByIdAsync(request.OwnerId, cancellationToken);
                if (company == null)
                    return new BaseRegistrationResponse { Success = false, Message = "Firma ikke fundet." };
                var CompanyCanReject = await _unitOfWork.ExpenseRegistrations.CanCompanyModerateAsync(registration.Id, company.Id, cancellationToken);
                if (!CompanyCanReject)
                    return new BaseRegistrationResponse { Success = false, Message = "Firmaet har ikke rettigheder til at afvise denne udgiftsregistrering." };
                registration.Reject(company, request.Comment);
                expense.Reject(company);
                await _unitOfWork.CompleteAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return new BaseRegistrationResponse { Success = true, Message = "Udgiftsregistrering er afvist." };
            }
            catch (InvalidOperationException ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new BaseRegistrationResponse { Success = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new BaseRegistrationResponse { Success = false, Message = "Der skete en fejl under afvisning af udgiftsregistreringen." };
            }
        }
    }
}
