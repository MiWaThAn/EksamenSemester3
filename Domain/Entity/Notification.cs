using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity
{
    public class Notification : Base
    {
        public Guid AccountId { get; private set; }
        public string Title { get; private set; }
        public string Message { get; private set; }
        public bool IsRead { get; private set; }
        public string? TargetType { get; private set; } //fks worklog eller ligende
        public Guid? TargetId { get; private set; } //id på worklog fx.

        private Notification() { }

        public Notification(Guid accountId, string title, string message, string? targetType = null, Guid? targetId = null)
        {
            AccountId = accountId;
            Title = title;
            Message = message;
            TargetType = targetType;
            TargetId = targetId;
            IsRead = false;
        }

        public void MarkAsRead()
        {
            IsRead = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
