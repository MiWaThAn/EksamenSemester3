using Application.DTO.External;
using Application.Interfaces;
using Application.Requests.ProjectActivities;
using Domain.Entity.Item;
using Domain.Entity.Item.Activities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Item.ProjectActivity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Requests.ProjectActivities
{
    public class GetProjectActivitiesForProjectQueryHandler : IRequestHandler<GetProjectActivitiesForProjectQuery, IEnumerable<ProjectActivityDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetProjectActivitiesForProjectQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ProjectActivityDto>> Handle(GetProjectActivitiesForProjectQuery request,CancellationToken cancellationToken)
        {
            return await _unitOfWork.ProjectActivities.GetQueryable()
                .AsNoTracking()
                .Include(pa => pa.Activity)
                .Where(pa => pa.ProjectId == request.ProjectId && pa.Status == Status.Åben)
                .Select(pa => new ProjectActivityDto
                {
                    ProjectActivityId = pa.Id,
                    ProjectId = pa.ProjectId,
                    ActivityId = pa.ActivityId,
                    ActivityName = pa.Activity.Name,
                    ActivityDescription = pa.Activity.Description,
                    Status = pa.Status.ToString(),
                    StartDate = pa.StartDate,
                    EndDate = pa.EndDate,
                    ResponsibleEmployeeId = pa.ResponsibleEmployeeId
                })
                .ToListAsync(cancellationToken);
        }
    }
}
