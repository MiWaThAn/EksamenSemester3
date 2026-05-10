using Application.Commands.Person.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Person
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
