using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Notification
{
    public record NotifyUserCommand(Guid AccountId, string Title, string Message) :IRequest;
}
