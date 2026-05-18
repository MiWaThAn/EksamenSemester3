using Application.Interfaces;
using Application.Interfaces.Services;
using Domain.Entity.Person;
using Domain.Guards;
using Domain.Interfaces.Item;
using MediatR;
using Shared.Person.Auth.Commands;
using Shared.Person.Auth.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Person.Handlers.Auth.Login
{
    public class PinLoginHandler(IUnitOfWork _unitOfWork, IHashingService _hashingService): IRequestHandler<PinLoginCommand, LoginResponse>
    {
        public async Task<LoginResponse> Handle(PinLoginCommand command,CancellationToken ct)
        {
            Guard.AgainstNull(command, nameof(command));
            Guard.AgainstNullOrEmpty(command.Pin, nameof(command.Pin));
            Guard.AgainstNullOrEmpty(command.AccountId, nameof(command.AccountId));
            bool Active = false;
            if (!Guid.TryParse(command.AccountId, out var accountId))
            {
                return new LoginResponse { Success = false, Message = "Ugyldigt konto-ID format." };
            }
            const string invalidCredentialsMessage = "Ugyldig PIN-kode eller konto.";
            try
            {
                await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, ct);
                Active = true;
                var user = await _unitOfWork.Accounts.GetByIdAsync(accountId);
                if (user == null)
                {
                    return new LoginResponse { Success = false, Message = invalidCredentialsMessage };
                }
                if (string.IsNullOrWhiteSpace(user.HashedPin))
                {
                    return new LoginResponse { Success = false, Message = invalidCredentialsMessage };
                }
                if (_hashingService.Verify(user.HashedPin, command.Pin))
                {
                    user.UpdateLastLogin(DateTime.UtcNow);
                    await _unitOfWork.CompleteAsync(ct);
                    await _unitOfWork.CommitTransactionAsync(ct);
                    return new LoginResponse { Success = true };
                }
                return new LoginResponse { Success = false, Message = invalidCredentialsMessage };
            }
            catch (Exception ex)
            {
                if (Active)
                    await _unitOfWork.RollbackTransactionAsync();
                return new LoginResponse { Success = false };
            }
        }
    }
}
