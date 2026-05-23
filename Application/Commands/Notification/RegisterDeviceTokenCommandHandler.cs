using Application.Interfaces;
using Domain.Entity.Person;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Notification
{
    public class RegisterDeviceTokenCommandHandler : IRequestHandler<RegisterDeviceTokenCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RegisterDeviceTokenCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(RegisterDeviceTokenCommand request, CancellationToken cancellationToken)
        {
            Guid AccountId = Guid.Parse(request.AccountId);
            var user = await _unitOfWork.Accounts.GetByIdAsync(AccountId, cancellationToken);

            if (user == null)
            {
                throw new Exception($"User {request.AccountId} not found.");
            }

            user.AddDeviceToken(request.DeviceToken);
            await _unitOfWork.CompleteAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
    }
}
