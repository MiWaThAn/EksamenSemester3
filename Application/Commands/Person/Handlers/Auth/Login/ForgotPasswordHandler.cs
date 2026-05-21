using Application.Commands.Account;
using Application.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.Account.Handlers
{
    internal class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ForgotPasswordHandler(IUnitOfWork uow)
        {
            _unitOfWork = uow;
        }

        public async Task Handle(ForgotPasswordCommand request, CancellationToken ct)
        {
            // 1. Find medarbejderen på deres e-mail (Brug jeres rigtige repo-metode)
            var employee = await _unitOfWork.Employees.GetByEmailAsync(request.Email);

            if (employee == null || employee.Account == null)
            {
                return; 
            }

            // 2. Generer token via jeres domænemetode på kontoen
            var token = employee.Account.GeneratePasswordResetToken();

            // 3. Gem token og udløb i databasen via UOW
            await _unitOfWork.CompleteAsync();

            // 4. Den simulerede e-mail logges i backenden
            var resetLink = $"https://localhost:7193/auth/reset-password?token={token}";

            Console.WriteLine("====================================================");
            Console.WriteLine($"SIMULERET EMAIL SENDT TIL: {request.Email}");
            Console.WriteLine($"KLIK HER FOR AT NULSTILLE KODEORD, BIG BOSS:");
            Console.WriteLine(resetLink);
            Console.WriteLine("====================================================");
        }
    }
}