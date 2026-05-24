using Application.Interfaces;
using Domain.Entity.Item.Registrations;
using MediatR;
using Shared.Item.Registrations.Commands.Time;
using Shared.Item.Registrations.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Item.Registrations.Time
{
    public class DeleteTimeRegistrationHandler : IRequestHandler<DeleteTimeRegistrationCommand, BaseRegistrationResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeleteTimeRegistrationHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<BaseRegistrationResponse> Handle(DeleteTimeRegistrationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, cancellationToken);
                var account = await _unitOfWork.Accounts.GetByIdAsync(request.AccountId, cancellationToken);
                if (account == null)
                    return new BaseRegistrationResponse { Success = false, Message = "Konto ikke fundet." };
                var registration = await _unitOfWork.HourRegistrations.GetByIdAsync(request.RegistrationId, cancellationToken);
                if (registration == null)
                    return new BaseRegistrationResponse { Success = false, Message = "Registrering ikke fundet." };
                if (registration.Status != RegistrationStatus.Godkendt)
                    _unitOfWork.HourRegistrations.DeleteByIdAsync(registration.Id);
                await _unitOfWork.CompleteAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return new BaseRegistrationResponse { Success = true, Message = "Registrering slettet." };
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
