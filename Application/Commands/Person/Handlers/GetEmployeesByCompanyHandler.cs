using Application.Commands.Person.Queries;
using Application.Interfaces;
using MediatR;
using Shared.Model;

namespace Application.Commands.Person.Handlers
{
    internal class GetEmployeesByCompanyHandler : IRequestHandler<GetEmployeesByCompanyQuery, IEnumerable<CompanyEmployeeModel>>
    {
        private readonly IUnitOfWork _uow;

        public GetEmployeesByCompanyHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<IEnumerable<CompanyEmployeeModel>> Handle(GetEmployeesByCompanyQuery request, CancellationToken ct)
        {
            var company = await _uow.Companies.GetWithEmployeesAsync(request.CompanyId);

            if (company == null || company.Employees == null)
                return Enumerable.Empty<CompanyEmployeeModel>();

            return company.Employees.Select(e => new CompanyEmployeeModel
            {
                Id = e.Id,
                FullName = e.Name
            });
        }
    }
}