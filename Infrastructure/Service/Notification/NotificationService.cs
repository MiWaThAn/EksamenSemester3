using Application.Services.Notification;
using Application.Services.Notification.Providers;
using Infrastructure.Service.Notification.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Service.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly IEnumerable<INotificationProvider> _providers;

        public NotificationService(IEnumerable<INotificationProvider> providers)
        {
            _providers = providers;
        }

        public async Task<bool> SendNotificationAsync(string recipient, string message, NotificationType type, CancellationToken ct = default)
        {
            // Pick the right provider based on type
            var provider = _providers.FirstOrDefault(p =>
                (type == NotificationType.Email && p is EmailNotificationProvider) ||
                (type == NotificationType.Sms && p is SmsNotificationProvider)||
                (type == NotificationType.PushNotification && p is PushNotificationService));

            var result = await provider?.SendAsync(recipient, message);
            return result==true;
        }
    }
}
