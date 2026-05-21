using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces.Notification
{
    public interface IPushNotificationService
    {
        Task<bool> SendAsync(string title, string body, string deviceToken, CancellationToken cancellationToken = default);
    }
}
