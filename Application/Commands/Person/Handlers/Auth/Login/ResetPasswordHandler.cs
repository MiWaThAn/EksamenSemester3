using Application.Commands.Account;
using Application.Interfaces;
using Application.Interfaces.Services;
using Domain.Interfaces.Item;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.Account.Handlers
{
    internal class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHashingService _hashingService;

        public ResetPasswordHandler(IUnitOfWork unitOfWork, IHashingService hashingService)
        {
            _unitOfWork = unitOfWork;
            _hashingService = hashingService;
        }

        public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken ct)
        {
            // 1. Hent kontoen fejlfrit på e-mailen (Hvis din model ligger i en property, så brug request.Model.Email)
            var account = await _unitOfWork.Accounts.GetByEmployeeEmailAsync(request.Email);

            if (account == null) return false;

            // 2. Hash det nye password sikkert med din BCrypt HashingService
            string hashedNewPassword = _hashingService.Hash(request.NewPassword);

            try
            {
                // 3. Kalder din ægte domænemetode i Account.cs, som tjekker din private token!
                account.ResetPassword(request.Token, hashedNewPassword);
            }
            catch (Exception)
            {
                return false; // Token var forkert eller udløbet!
            }

            // 4. Gemmer ændringerne i databasen via din Unit of Work
            await _unitOfWork.CompleteAsync();

            return true;
        }
    }
}