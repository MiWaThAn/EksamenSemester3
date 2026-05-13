using Application.DTOs;
using MediatR;
using Shared.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Person.Queries
{
    public record GetAllEmployeesQuery() : IRequest<IEnumerable<CompanyEmployeeModel>>;
}
