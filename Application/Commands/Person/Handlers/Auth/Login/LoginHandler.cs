using Application.Interfaces;
using Application.Interfaces.Services;
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
    public class LoginHandler(IUnitOfWork _unitOfWork, IHashingService _hashingService,ITokenService _tokenService): IRequestHandler<LoginCommand, LoginResponse>
    {
        public async Task<LoginResponse> Handle(LoginCommand command, CancellationToken ct)
        {
            Guard.AgainstNullOrEmpty(command.Username, nameof(command.Username));
            Guard.AgainstNullOrEmpty(command.Password, nameof(command.Password));
            const string invalidCredentialsMessage = "Ugyldigt brugernavn eller adgangskode.";
            bool Active = false;
            try
            {
                await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, ct);
                Active = true;
                var user = await _unitOfWork.Accounts.GetByUsernameAsync(command.Username,ct);
                if(user == null)
                {
                    return new LoginResponse { Success = false,Message = invalidCredentialsMessage };
                }
                if(_hashingService.Verify(user.HashedPassword, command.Password))
                {
                    user.UpdateLastLogin(DateTime.UtcNow);
                    await _unitOfWork.CompleteAsync(ct);
                    await _unitOfWork.CommitTransactionAsync(ct);
                    var token = _tokenService.GetToken(user);
                    return new LoginResponse { Success = true, Token=token};
                }
                return new LoginResponse { Success = false, Message = invalidCredentialsMessage };
            }
            catch(Exception ex)
            {
                if (Active)
                    await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
