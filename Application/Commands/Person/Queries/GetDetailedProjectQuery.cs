using MediatR;
using Shared.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Person.Queries
{
    public class GetDetailedProjectQuery : IRequest<DetailedProjectModel>
    {
        public Guid ProjectId { get; }
        public Guid AccountId { get; }

        public GetDetailedProjectQuery(Guid projectId, Guid accountId)
        {
            ProjectId = projectId;
            AccountId = accountId;
        }
    }
}
