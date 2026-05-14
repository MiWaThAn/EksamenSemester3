using MediatR;
using Shared.Person.Auth.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Person.Auth.Commands
{
    public record RegisterEmployeeAccountCommand(Guid EmployeeId, string Password, string Username, string PhoneNumber) : IRequest<RegisterEmployeeAccountResponse>;

}
