using MediatR;
using Shared.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Person.Queries
{
    public class GetDetailedProjectQuery : IRequest<DetailedProjectModel>
    {
        public Guid ProjectId { get; set; }

        public GetDetailedProjectQuery(Guid projectId)
        {
            ProjectId = projectId;
        }
    }
}
