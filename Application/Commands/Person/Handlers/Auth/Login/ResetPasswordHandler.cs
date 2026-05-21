using Application.Commands.Account;
using Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.Account.Handlers
{
    internal class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ResetPasswordHandler(IUnitOfWork uow)
        {
            _unitOfWork = uow;
        }

        public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken ct)
        {
            // 1. Find kontoen baseret på jeres RecoveryToken (Brug jeres rigtige repo-metode)
            var account = await _unitOfWork.Accounts.GetByRecoveryTokenAsync(request.Token);

            if (account == null)
            {
                return false; // Token matcher ikke eller findes ikke, legend!
            }

            // 2. Hash det nye kodeord (Udskift med jeres rigtige hashing-tjeneste, f.eks. BCrypt eller PasswordHasher!)
            string hashedNewPassword = YourPasswordHasher.HashPassword(request.NewPassword);

            // 3. Kald jeres domænemetode direkte på kontoen
            account.ResetPassword(request.Token, hashedNewPassword);

            // 4. Gem det opdaterede kodeord i databasen via UOW
            await _unitOfWork.CompleteAsync();

            return true;
        }
    }
}