using Application.Services.Notification.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Service.Notification.Providers
{
    public class EmailNotificationProvider : INotificationProvider
    {
        public async Task<bool> SendAsync(string recipient, string message)
        {
            Console.WriteLine($"Sending Email to {recipient}: {message}");
            return await Task.FromResult(true);
        }
    }
}
