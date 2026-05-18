using Application.Interfaces;
using Application.Interfaces.Services;
using Domain.Entity.Person.Auth;
using Domain.Interfaces.Item;
using MediatR;
using Shared.Person.Auth.Commands;
using Shared.Person.Auth.Responses;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using static System.Net.WebRequestMethods;

namespace Application.Commands.Person.Handlers.Auth.Registration
{
    public class RegisterPincodeHandler(IUnitOfWork _unitOfWork,IHashingService _hashingService,ITokenService _tokenService) : IRequestHandler<RegisterAccountPinCommand, RegisterAccountPinResponse>
    {
        public async Task<RegisterAccountPinResponse> Handle(RegisterAccountPinCommand command, CancellationToken ct)
        {
            bool Active = false;
            if (!Guid.TryParse(command.AccountId, out var accountId))
            {
                return new RegisterAccountPinResponse { Success = false, Message = "Ugyldigt konto-ID format." };
            }
            try
            {
                await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, ct);
                Active = true;
                var account = await _unitOfWork.Accounts.GetByIdAsync(accountId);
                if (account == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new RegisterAccountPinResponse { Success = false, Message = "Konto kunne ikke findes" };
                }
                var pin = _hashingService.Hash(command.Pin);
                account.UpdateHashedPin(pin);
                await _unitOfWork.CompleteAsync(ct);
                await _unitOfWork.CommitTransactionAsync(ct);
                var token = _tokenService.GetToken(account);
                return new RegisterAccountPinResponse { Token = token, Success = true, Message = "Pinkode oprettet" };
            }
            catch (Exception)
            {
                if(Active)
                    await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
