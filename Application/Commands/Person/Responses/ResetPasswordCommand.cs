using MediatR;

namespace Application.Commands.Account
{
    public record ResetPasswordCommand(string Token, string NewPassword) : IRequest<bool>;
}