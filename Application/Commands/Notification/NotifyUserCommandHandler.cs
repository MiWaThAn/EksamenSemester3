using Application.Interfaces;
using Domain.Interfaces.Notification;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Notification
{
    public class NotifyUserCommandHandler
    {
        private readonly IPushNotificationService _pushService;
        private readonly IUnitOfWork _unitOfWork;

        public NotifyUserCommandHandler(IPushNotificationService pushService, IUnitOfWork unitOfWork)
        {
            _pushService = pushService;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(NotifyUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Accounts.GetByIdAsync(request.AccountId);
            foreach(string token in user.DeviceTokens.Select(t=>t.Value))
            {
                await _pushService.SendAsync(request.Title, request.Message, token, cancellationToken);
            }
        }
    }
}
