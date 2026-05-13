using Application.Commands.Person.Queries;
using Application.DTOs;
using Application.Interfaces;
using MediatR;
using Domain.Builders.Person;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Person.Handlers
{
    internal class GetCompanyByIdHandler : IRequestHandler<GetCompanyByIdQuery, CompanyDTO>
    {
        private readonly IUnitOfWork _uow;

        public GetCompanyByIdHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }
        // takes company and the employees connected to it
        public async Task<CompanyDTO> Handle(GetCompanyByIdQuery request, CancellationToken ct)
        {
            var company = await _uow.Companies.GetWithEmployeesAsync(request.Id);

            if (company == null) return null;

            return CompanyDTO.FromEntity(company);
        }

    }
}
