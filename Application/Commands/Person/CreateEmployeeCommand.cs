using Application.Commands.Person.Responses;
using Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Person
{
    public record CreateEmployeeCommand(
            string Name,
            string Email,
            int EmployeeType,
            bool IsAutonomous,
            Guid CompanyId
        ) : IRequest<EmployeeDTO>;
}
