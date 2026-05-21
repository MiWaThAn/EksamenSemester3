using MediatR;
using System;

namespace Application.Commands.Person
{
    public record UpdateEmployeeDetailsCommand(
        Guid EmployeeId,
        string FullName,
        string Email,
        string MobileNumber
    ) : IRequest<bool>;
}