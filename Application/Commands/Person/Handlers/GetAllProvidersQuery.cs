using Application.DTO.External;
using MediatR;
using Shared.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Person.Handlers
{
    public record GetAllProvidersQuery : IRequest<IEnumerable<ProviderModel>>;
}
