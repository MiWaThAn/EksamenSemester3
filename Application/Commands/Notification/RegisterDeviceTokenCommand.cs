using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Notification
{
    public record RegisterDeviceTokenCommand(string AccountId, string DeviceToken) : IRequest;
}
