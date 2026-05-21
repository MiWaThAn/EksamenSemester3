using Domain.Entity.Person;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services.Notification
{
    public interface INotificationService
    {
        Task<bool> SendNotificationAsync(string recipient, string message, NotificationType type, CancellationToken ct = default);
    }
}
