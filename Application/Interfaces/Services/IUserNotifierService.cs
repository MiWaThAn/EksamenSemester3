using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services
{
    public interface IUserNotifierService
    {
        Task NotifyUserAsync(
            Guid userId,
            string title,
            string body,
            Dictionary<string, string> payload = null,
            CancellationToken cancellationToken = default);
    }
}
