using Application.Interfaces;
using Domain.Entity;
using Domain.Interfaces.Notification;
using MediatR;
using Shared.Item.Registrations.Events.Worklogs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Event
{
    public class WorkLogApprovedNotificationHandler : INotificationHandler<WorkLogApprovedEvent>
    {
        private readonly IPushNotificationService _pushService;
        private readonly IUnitOfWork _unitOfWork;

        public WorkLogApprovedNotificationHandler(IPushNotificationService pushService, IUnitOfWork unitOfWork)
        {
            _pushService = pushService;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(WorkLogApprovedEvent notification, CancellationToken cancellationToken)
        {
            var dbNotification = new Notification(
                notification.EmployeeAccountId,
            title:"Din arbejdslog er blevet godkendt",
            message:$"Din arbejdslog for den {notification.DateString} er blevet godkendt af ejeren.",
              targetType: "WorkLog",
                targetId: notification.WorkLogId
            );

            await _unitOfWork.Notifications.AddAsync(dbNotification, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            var employee = await _unitOfWork.Accounts.GetByIdAsync(notification.EmployeeAccountId, cancellationToken);
            if (employee != null && employee.DeviceTokens.Any())
            {
                var tasks = employee.DeviceTokens.Select(t =>
                    _pushService.SendAsync(dbNotification.Title, dbNotification.Message, t.Value, cancellationToken));

                try
                {
                    await Task.WhenAll(tasks);
                }
                catch (Exception)
                {

                }
            }
        }
    }
}
