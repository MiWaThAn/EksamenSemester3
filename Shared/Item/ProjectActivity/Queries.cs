using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Item.ProjectActivity
{
    public record GetProjectActivitiesForProjectQuery(Guid ProjectId) : IRequest<IEnumerable<ProjectActivityDto>>;
}
