using Application.DTO.External;
using Application.DTOs;
using MediatR;
using Shared.Item.ProjectActivity;
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
    public class GetCompanyProjectsByEmployeeAccountId : IRequest<IEnumerable<ProjectDto>>
    {
        public Guid AccountId { get; }

        public GetCompanyProjectsByEmployeeAccountId(Guid accountId)
        {
            AccountId = accountId;
        }
    }
}
