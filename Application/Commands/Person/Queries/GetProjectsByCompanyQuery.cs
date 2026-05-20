using MediatR;
using Shared.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Person.Queries
{
    public class GetProjectsByCompanyQuery : IRequest<IEnumerable<CompanyProjectModel>>
    {
        public Guid CompanyId { get; }

        public GetProjectsByCompanyQuery(Guid companyId)
        {
            CompanyId = companyId;
        }
    }
}
