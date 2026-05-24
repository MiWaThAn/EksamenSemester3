using Application.Interfaces;
using MediatR;
using Shared.Item.Registrations.Commands;
using Shared.Item.Registrations.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Item.Registrations
{
    public class UpdateRegistrationDescriptionHandler : IRequestHandler<UpdateRegistrationDescriptionCommand, BaseRegistrationResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateRegistrationDescriptionHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<BaseRegistrationResponse> Handle(UpdateRegistrationDescriptionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, cancellationToken);
                var TimeRegistration = await _unitOfWork.HourRegistrations.GetByIdAsync(request.RegistrationId, cancellationToken);
                var ExpenseRegistration = await _unitOfWork.ExpenseRegistrations.GetByIdAsync(request.RegistrationId, cancellationToken);
                if (TimeRegistration != null)
                {
                    TimeRegistration.UpdateDescription(request.NewDescription);
                    await _unitOfWork.CompleteAsync(cancellationToken);
                    return BaseRegistrationResponse.Ok(TimeRegistration.Id);
                }
                else if (ExpenseRegistration != null)
                {
                    ExpenseRegistration.UpdateDescription(request.NewDescription);
                    await _unitOfWork.CompleteAsync(cancellationToken);
                    return BaseRegistrationResponse.Ok(ExpenseRegistration.Id);
                }
                return BaseRegistrationResponse.Fail("Registration not found.");
            }
            catch (Exception ex)
            {
                return BaseRegistrationResponse.Fail(ex.Message);
            }
        }
    }
}
