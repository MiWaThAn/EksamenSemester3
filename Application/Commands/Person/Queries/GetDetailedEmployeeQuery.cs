using MediatR;
using Shared.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Person.Queries
{
    public class GetDetailedEmployeeQuery : IRequest<DetailedEmployeeModel>
    {
        public Guid EmployeeId { get; set; }

        public GetDetailedEmployeeQuery(Guid employeeId)
        {
            EmployeeId = employeeId;
        }
    }
}
