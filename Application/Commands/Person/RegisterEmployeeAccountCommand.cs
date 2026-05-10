using Application.Commands.Person.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Person
{
    public record RegisterEmployeeAccountCommand(
Guid EmployeeId,
string Password,
string Username,
string PhoneNumber
) : IRequest<RegisterEmployeeAccountResponse>;

}
