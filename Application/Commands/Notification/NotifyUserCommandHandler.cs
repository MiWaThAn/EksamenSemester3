using Application.Interfaces;
using Domain.Interfaces.Notification;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.Notification
{
    public class NotifyUserCommandHandler : IRequestHandler<NotifyUserCommand>
    {
        private readonly IPushNotificationService _pushService;
        private readonly IUnitOfWork _unitOfWork;

        public NotifyUserCommandHandler(IPushNotificationService pushService, IUnitOfWork unitOfWork)
        {
            _pushService = pushService;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(NotifyUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Accounts.GetByIdAsync(request.AccountId, cancellationToken);

            if (user == null || !user.DeviceTokens.Any())
            {
                return;
            }

            var pushTasks = user.DeviceTokens.Select(async token =>
            {
                try
                {
                    await _pushService.SendAsync(request.Title, request.Message, token.Value, cancellationToken);
                }
                catch (Exception ex)
                {
                }
            });

            await Task.WhenAll(pushTasks);
        }
    }
}