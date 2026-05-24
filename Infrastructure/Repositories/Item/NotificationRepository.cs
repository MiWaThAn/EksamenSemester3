using Application.Interfaces.Repo.Item;
using Domain.Entity;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories.Item
{
    public class NotificationRepository : GenericRepository<Notification>,INotificationRepository
    {
        public NotificationRepository(AppDbContext context) : base(context) { }
    }
}
