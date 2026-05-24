using Application.Commands.Person.Queries;
using Application.Interfaces;
using MediatR;
using Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.Person.Handlers
{
    public class GetDetailedProjectHandler : IRequestHandler<GetDetailedProjectQuery, DetailedProjectModel?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDetailedProjectHandler(IUnitOfWork uow)
        {
            _unitOfWork = uow;
        }

        public async Task<DetailedProjectModel?> Handle(GetDetailedProjectQuery request, CancellationToken ct)
        {
            var project = await _unitOfWork.Projects.GetByIdWithDetailsAsync(request.ProjectId);

            if (project == null)
                return null;

            var relatedEmployees = await _unitOfWork.Employees.GetEmployeesRelatedToProjectAsync(request.ProjectId);

            return new DetailedProjectModel
            {
                Id = project.Id,
                ProjectName = project.Name,
                Description = project.Description ?? "Ingen beskrivelse tilgængelig.",
                Status = project.Status.ToString() ?? "Åben",
                CustomerName = project.Customer?.Name ?? "Ikke angivet",

                Activities = project.Activities?.Select(a => new ProjectActivityModel
                {
                    Id = a.Id,

                    ActivityName = a.Activity?.Name ?? "Navnløs aktivitet",

                    ActivityNumber = a.ActivityId.ToString().Substring(0, 5).ToUpper(),

                    Status = a.Status.ToString(),
                    StatusText = a.Status.ToString(),

                    TimeEstimate = (a.EndDate > a.StartDate)
                        ? Math.Round((a.EndDate - a.StartDate).TotalHours, 1)
                        : 0.0
                }).ToList() ?? new List<ProjectActivityModel>(),

                Employees = relatedEmployees?.Select(e => new CompanyEmployeeModel
                {
                    Id = e.Id,
                    FullName = e.Name
                }).ToList() ?? new List<CompanyEmployeeModel>()
            };
        }
    }
}