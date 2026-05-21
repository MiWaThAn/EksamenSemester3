using MediatR;

namespace Application.Commands.Account
{
    public record ForgotPasswordCommand(string Email) : IRequest;
}