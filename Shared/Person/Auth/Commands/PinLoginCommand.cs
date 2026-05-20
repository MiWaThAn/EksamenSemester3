using MediatR;
using Shared.Person.Auth.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Person.Auth.Commands
{
    public record PinLoginCommand(string Pin, string AccountId) : IRequest<LoginResponse>;
}
