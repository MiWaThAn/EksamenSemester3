using Application.Interfaces;
using Application.Interfaces.Services;
using Domain.Interfaces.Notification;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Notification.Service
{
    public class UserNotifierService : IUserNotifierService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPushNotificationService _pushService;

        public UserNotifierService(IUnitOfWork unitOfWork, IPushNotificationService pushService)
        {
            _unitOfWork = unitOfWork;
            _pushService = pushService;
        }

        public async Task NotifyUserAsync(Guid userId, string title, string body, Dictionary<string, string> payload = null, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Accounts.GetByIdAsync(userId, cancellationToken);
            if (user == null || !user.DeviceTokens.Any())
            {
                return;
            }
            foreach (var token in user.DeviceTokens)
            {
                await _pushService.SendAsync(title, body, token.Value, cancellationToken);
            }
        }
    }
}
