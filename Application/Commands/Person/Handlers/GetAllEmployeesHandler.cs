using Application.Commands.Person.Queries;
using Application.DTOs;
using Application.Interfaces;
using MediatR;

namespace Application.Commands.Person.Handlers
{
    internal class GetAllEmployeesHandler : IRequestHandler<GetAllEmployeesQuery, IEnumerable<EmployeeDTO>>
    {
        private readonly IUnitOfWork _uow;

        public GetAllEmployeesHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<IEnumerable<EmployeeDTO>> Handle(GetAllEmployeesQuery request, CancellationToken ct)
        {
            var employees = await _uow.Employees.GetAllAsync();

            // Vi bruger din FromEntity metode på hver enkelt i listen
            return employees.Select(e => EmployeeDTO.FromEntity(e));
        }
    }
}