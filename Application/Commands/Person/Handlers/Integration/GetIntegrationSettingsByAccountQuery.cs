using MediatR;
using Shared.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Person.Handlers.Integration
{
    public record GetIntegrationSettingsByAccountQuery(Guid AccountId)
    : IRequest<IEnumerable<IntegrationSettingModel>>;
}
