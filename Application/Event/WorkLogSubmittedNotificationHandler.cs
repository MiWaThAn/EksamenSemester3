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
    public class WorkLogSubmittedNotificationHandler : INotificationHandler<WorkLogSubmittedEvent>
    {
        private readonly IPushNotificationService _pushService;
        private readonly IUnitOfWork _unitOfWork;

        public WorkLogSubmittedNotificationHandler(IPushNotificationService pushService, IUnitOfWork unitOfWork)
        {
            _pushService = pushService;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(WorkLogSubmittedEvent notificationEvent, CancellationToken cancellationToken)
        {
            var dbNotification = new Notification(
                notificationEvent.OwnerAccountId,
                "Ny arbejdslog afventer godkendelse",
                "En medarbejder har indsendt sin arbejdslog for i dag.",
                targetType: "WorkLogReview",
                targetId: notificationEvent.WorkLogId
            );

            await _unitOfWork.Notifications.AddAsync(dbNotification, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            var owner = await _unitOfWork.Accounts.GetByIdAsync(notificationEvent.OwnerAccountId, cancellationToken);
            if (owner != null && owner.DeviceTokens.Any())
            {
                var tasks = owner.DeviceTokens.Select(t =>
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
