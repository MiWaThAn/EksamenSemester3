using MediatR;
using Shared.Model;

namespace Application.Commands.Person.Queries
{
    public class GetProjectsByEmployeeQuery : IRequest<IEnumerable<CompanyProjectModel>>
    {
        public Guid AccountId { get; set; }

        public GetProjectsByEmployeeQuery() { }

        public GetProjectsByEmployeeQuery(Guid accountId)
        {
            AccountId = accountId;
        }
    }
}