using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services.Notification.Providers
{
    public interface INotificationProvider
    {
        Task<bool> SendAsync(string recipient, string message);
    }
}
