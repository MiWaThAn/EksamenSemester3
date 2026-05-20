using Application.Commands.Person.Queries;
using Application.Interfaces;
using MediatR;
using Shared.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.Person.Handlers
{
    internal class GetDetailedProjectHandler : IRequestHandler<GetDetailedProjectQuery, DetailedProjectModel?>
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
                Employees = relatedEmployees?.Select(e => new CompanyEmployeeModel
                {
                    Id = e.Id,
                    FullName = e.Name
                }).ToList() ?? new List<CompanyEmployeeModel>()
            };
        }
    }
}