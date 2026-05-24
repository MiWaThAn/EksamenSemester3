using MediatR;

namespace Application.Commands.Account
{
    public record ResetPasswordCommand(string Email, string Token, string NewPassword) : IRequest<bool>;
}