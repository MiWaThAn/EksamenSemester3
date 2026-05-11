using Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Person.Queries
{
    public record GetEmployeesByCompanyQuery(Guid CompanyId) : IRequest<IEnumerable<EmployeeDTO>>;
}
