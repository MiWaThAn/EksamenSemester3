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
    public class WorkLogRejectedNotificationHandler : INotificationHandler<WorkLogRejectedEvent>
    {
        private readonly IPushNotificationService _pushService;
        private readonly IUnitOfWork _unitOfWork;

        public WorkLogRejectedNotificationHandler(IPushNotificationService pushService, IUnitOfWork unitOfWork)
        {
            _pushService = pushService;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(WorkLogRejectedEvent notificationEvent, CancellationToken cancellationToken)
        {
            var dbNotification = new Notification(
                notificationEvent.EmployeeAccountId,
                "Arbejdslog kræver handling",
                $"Din arbejdslog er blevet afvist med begrundelsen: \"{notificationEvent.Reason}\"",
                targetType: "WorkLog",
                targetId: notificationEvent.WorkLogId
            );

            await _unitOfWork.Notifications.AddAsync(dbNotification, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            var employee = await _unitOfWork.Accounts.GetByIdAsync(notificationEvent.EmployeeAccountId, cancellationToken);
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
