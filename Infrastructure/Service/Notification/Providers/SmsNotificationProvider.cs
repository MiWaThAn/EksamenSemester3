using Application.Services.Notification.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Service.Notification.Providers
{
    public class SmsNotificationProvider : INotificationProvider
    {
        public async Task<bool> SendAsync(string recipient, string message)
        {
            Console.WriteLine($"Sending SMS to {recipient}: {message}");
            return await Task.FromResult(true);
        }
    }
}
