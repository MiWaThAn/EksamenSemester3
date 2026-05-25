using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Person.Handlers.Integration
{
    public record DeleteIntegrationSettingCommand(Guid SettingId, Guid AccountId) : IRequest;
}
