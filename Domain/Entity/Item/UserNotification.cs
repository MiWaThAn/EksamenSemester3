using Domain.Entity.Person;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity.Item
{
    public class UserNotification : Base
    {
        public Guid AccountId { get; private set; }
        public string Title { get; private set; }
        public string Message { get; private set; }
        public bool IsRead { get; private set; }
        public DateTime CreatedAt { get; private set; }

        internal UserNotification(Account account, string title, string message)
        {
            AccountId = account.Id;
            Title = title;
            Message = message;
            IsRead = false;
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkAsRead()
        {
            IsRead = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
