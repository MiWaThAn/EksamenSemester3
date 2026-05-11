using Application.Commands.Person.Queries;
using Application.DTOs;
using Application.Interfaces;
using MediatR;

namespace Application.Commands.Person.Handlers
{
    internal class GetEmployeesByCompanyHandler : IRequestHandler<GetEmployeesByCompanyQuery, IEnumerable<EmployeeDTO>>
    {
        private readonly IUnitOfWork _uow;

        public GetEmployeesByCompanyHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<IEnumerable<EmployeeDTO>> Handle(GetEmployeesByCompanyQuery request, CancellationToken ct)
        {
            var company = await _uow.Companies.GetWithEmployeesAsync(request.CompanyId);

            if (company == null || company.Employees == null)
                return Enumerable.Empty<EmployeeDTO>();

            return company.Employees.Select(e => EmployeeDTO.FromEntity(e));
        }
    }
}