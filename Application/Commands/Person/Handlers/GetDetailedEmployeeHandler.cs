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

        // TODO: Add phone number to employee via account // also maybe add caching if it doesnt take too long
        public async Task<DetailedEmployeeModel?> Handle(GetDetailedEmployeeQuery request, CancellationToken ct)
        {
            var employee = await _unitOfWork.Employees.GetByIdWithAccountAsync(request.EmployeeId);

            if (employee == null)
                return null;

            var relatedProjects = await _unitOfWork.Projects.GetProjectsRelatedToEmployeeAsync(request.EmployeeId);

            return new DetailedEmployeeModel
            {
                Id = employee.Id,
                FullName = employee.Name,
                Email = employee.Email?.Value ?? string.Empty,
                MobileNumber = employee.Account?.PhoneNumber?.Value ?? string.Empty,
                Projects = relatedProjects?.Select(p => new CompanyProjectModel
                {
                    Id = p.Id,
                    ProjectName = p.Name
                }).ToList() ?? new List<CompanyProjectModel>()
            };
        }
    }
}