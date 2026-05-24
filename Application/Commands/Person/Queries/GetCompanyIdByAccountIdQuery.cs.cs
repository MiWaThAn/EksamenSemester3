using MediatR;
using System;

namespace Application.Commands.Person.Queries
{
    public record GetCompanyIdByAccountIdQuery(Guid AccountId) : IRequest<Guid>;
}