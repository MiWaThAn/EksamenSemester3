using MediatR;
using Shared.Model;

namespace Application.Commands.Person.Queries
{
    public class GetEmployeesByCompanyQuery : IRequest<IEnumerable<CompanyEmployeeModel>>
    {
        public Guid CompanyId { get;}
        public Guid AccountId { get;}

        public GetEmployeesByCompanyQuery(Guid companyId, Guid accountId)
        {
            CompanyId = companyId;
            AccountId = accountId;
        }
    }
}