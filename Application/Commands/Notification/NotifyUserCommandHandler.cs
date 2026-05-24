using Application.Interfaces;
using Domain.Interfaces.Notification;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.Notification
{
    // 1. Husk MediatR interfacet
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
            // 2. Husk cancellationToken
            var user = await _unitOfWork.Accounts.GetByIdAsync(request.AccountId, cancellationToken);

            if (user == null || !user.DeviceTokens.Any())
            {
                return; // Ingenting at gøre
            }

            // 3. Kør alle netværkskald parallelt og fang fejl på individuelle tokens
            var pushTasks = user.DeviceTokens.Select(async token =>
            {
                try
                {
                    await _pushService.SendAsync(request.Title, request.Message, token.Value, cancellationToken);
                }
                catch (Exception ex)
                {
                    // TODO: Log fejlen (f.eks. "Kunne ikke sende til token X for bruger Y")
                    // Vi kaster IKKE fejlen videre, for så stopper de andre notifikationer
                }
            });

            await Task.WhenAll(pushTasks);
        }
    }
}