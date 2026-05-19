using Application.Commands.Person.Queries;
using Application.Interfaces;
using MediatR;
using Shared.Model;
namespace Application.Commands.Person.Handlers
{
    internal class GetDetailedEmployeeHandler : IRequestHandler<GetDetailedEmployeeQuery, DetailedEmployeeModel?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDetailedEmployeeHandler(IUnitOfWork uow)
        {
            _unitOfWork = uow;
        }

        public async Task<DetailedEmployeeModel?> Handle(GetDetailedEmployeeQuery request, CancellationToken ct)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(request.EmployeeId);

            if (employee == null)
                return null;

            var relatedProjects = await _unitOfWork.Projects.GetProjectsRelatedToEmployeeAsync(request.EmployeeId);

            return new DetailedEmployeeModel
            {
                Id = employee.Id,
                FullName = employee.Name,
                Email = employee.Email?.Value ?? string.Empty,
                Projects = relatedProjects?.Select(p => new CompanyProjectModel
                {
                    Id = p.Id,
                    ProjectName = p.Name
                }).ToList() ?? new List<CompanyProjectModel>()
            };
        }
    }
}