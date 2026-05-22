using Application.Commands.Account;
using Application.Interfaces;
using Application.Interfaces.Services;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.Account.Handlers
{
    internal class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordResetEmailService _emailService;

        public ForgotPasswordHandler(IUnitOfWork uow, IPasswordResetEmailService emailservice)
        {
            _unitOfWork = uow;
            _emailService = emailservice;
        }

        public async Task Handle(ForgotPasswordCommand request, CancellationToken ct)
        {
            var account = await _unitOfWork.Accounts.GetByEmployeeEmailAsync(request.Email);

            if (account == null) return;

            string token = account.GeneratePasswordResetToken();
            await _unitOfWork.CompleteAsync();

            await _emailService.SendPasswordResetEmailAsync(request.Email, token);

            Console.WriteLine($"Mail sendt til: {account.Username}");
        }

    }
}