using Application.Commands.Person.Queries;
using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.Repo.Person;
using MediatR;
using Shared.Model;

namespace Application.Commands.Person.Handlers
{
    public class GetAllEmployeesHandler : IRequestHandler<GetAllEmployeesQuery, IEnumerable<CompanyEmployeeModel>>
    {
        private readonly IEmployeeRepository _repository;

        public GetAllEmployeesHandler(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CompanyEmployeeModel>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
        {
            var employees = await _repository.GetAllAsync();

            return employees.Select(e => new CompanyEmployeeModel
            {
                Id = e.Id,
                FullName = e.Name,
                IsSelected = false,
                NotificationCount = 0
            });
        }
    }
}