using MediatR;
using Shared.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Person.Queries
{
    public class GetDetailedEmployeeQuery : IRequest<DetailedEmployeeModel>
    {
        public Guid EmployeeId { get; }
        public Guid AccountId { get; }

        public GetDetailedEmployeeQuery(Guid employeeId, Guid accountId)
        {
            EmployeeId = employeeId;
            AccountId = accountId;
        }
    }
}
