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
    public class GetProjectsByCompanyHandler : IRequestHandler<GetProjectsByCompanyQuery, IEnumerable<CompanyProjectModel>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetProjectsByCompanyHandler(IUnitOfWork uow)
        {
            _unitOfWork = uow;
        }

        public async Task<IEnumerable<CompanyProjectModel>> Handle(GetProjectsByCompanyQuery request, CancellationToken ct)
        {
            // Vi henter firmaet og dets projekter via din Unit of Work
            var company = await _unitOfWork.Companies.GetWithProjectsAsync(request.CompanyId);

            if (company == null || company.Projects == null)
                return Enumerable.Empty<CompanyProjectModel>();

            // Vi mapper databasens projekter over i din CompanyProjectModel
            return company.Projects.Select(p => new CompanyProjectModel
            {
                Id = p.Id,
                ProjectName = p.Name,
                IsSelected = false,
                NotificationCount = 0
            });
        }
    }
}