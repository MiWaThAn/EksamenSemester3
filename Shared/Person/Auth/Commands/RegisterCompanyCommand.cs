using MediatR;
using Shared.Person.Auth.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Person.Auth.Commands
{
    public record RegisterCompanyCommand(
        string CompanyName,
        string Password,
        string Username,
        string Email,
        string PhoneNumber,
        string CVRNumber
        ) : IRequest<RegisterCompanyResponse>;
}
