using Application.Commands.Person.Queries;
using Application.DTO.External;
using Application.Interfaces;
using Domain.Entity.Item;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Item.ProjectActivity;
using Shared.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Requests.Projects
{
    public class GetCompanyProjectsByEmployeeAccountIdHandler : IRequestHandler<GetCompanyProjectsByEmployeeAccountId, IEnumerable<ProjectDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCompanyProjectsByEmployeeAccountIdHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<ProjectDto>> Handle(GetCompanyProjectsByEmployeeAccountId request, CancellationToken cancellationToken)
        {
            var account = await _unitOfWork.Accounts.GetByIdAsync(request.AccountId, cancellationToken);
            if(account == null)
                return Enumerable.Empty<ProjectDto>();
            if(!account.EmployeeId.HasValue)
                return Enumerable.Empty<ProjectDto>();
            var employee = await _unitOfWork.Employees.GetByIdAsync(account.EmployeeId.Value, cancellationToken);
            if(employee == null)
                return Enumerable.Empty<ProjectDto>();
            return await _unitOfWork.Projects.GetQueryable()
                .AsNoTracking()
                .Where(pa => pa.CompanyId == employee.CompanyId && pa.Status == Status.Åben)
                .Select(pa => new ProjectDto()
                {
                    ProjectId = pa.Id,
                    ProjectName = pa.Name,
                    Description = pa.Description,
                    Status = pa.Status.ToString(),
                    StartDate = pa.CreatedAt,
                    ResponsibleEmployeeId = pa.ResponsibleEmployeeId
                })
                .ToListAsync(cancellationToken);
        }
    }
}
