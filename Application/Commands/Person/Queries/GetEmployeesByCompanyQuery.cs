using MediatR;
using Shared.Model;

namespace Application.Commands.Person.Queries
{
    public class GetEmployeesByCompanyQuery : IRequest<IEnumerable<CompanyEmployeeModel>>
    {
        public Guid CompanyId { get; }

        public GetEmployeesByCompanyQuery(Guid companyId)
        {
            CompanyId = companyId;
        }
    }
}