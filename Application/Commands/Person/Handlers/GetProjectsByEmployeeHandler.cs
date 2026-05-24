using Application.Commands.Person.Queries;
using Application.Interfaces;
using Domain.Interfaces.Repos; // Ret til jeres rigtige namespace for IUnitOfWork
using MediatR;
using Shared.Model; // Gør det muligt at finde CompanyProjectModel
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.Person.Handlers
{
    public class GetProjectsByEmployeeHandler : IRequestHandler<GetProjectsByEmployeeQuery, IEnumerable<CompanyProjectModel>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetProjectsByEmployeeHandler(IUnitOfWork uow)
        {
            _unitOfWork = uow;
        }

        public async Task<IEnumerable<CompanyProjectModel>> Handle(GetProjectsByEmployeeQuery request, CancellationToken ct)
        {
            var account = await _unitOfWork.Accounts.GetByIdAsync(request.AccountId, ct);

            if (account == null)
                throw new UnauthorizedAccessException("Konto ikke fundet.");

            if (!account.EmployeeId.HasValue)
                throw new UnauthorizedAccessException("Kun medarbejdere kan hente personlige projekter.");

            var employeeId = account.EmployeeId.Value;

            var projects = await _unitOfWork.Projects.GetProjectsRelatedToEmployeeAsync(employeeId, ct);

            if (projects == null)
                return Enumerable.Empty<CompanyProjectModel>();

            return projects.Select(p => new CompanyProjectModel
            {
                Id = p.Id,
                ProjectName = p.Name
            });
        }
    }
}