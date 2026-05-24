using MediatR;
using Shared.Item.Registrations.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Item.Registrations.Commands
{
    public record UpdateRegistrationDescriptionCommand(
        Guid AccountId,
        Guid RegistrationId,
        string NewDescription
    ) : IRequest<BaseRegistrationResponse>;
}
