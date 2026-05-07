using Application.DTOs;
using Application.Interfaces;
using Domain.Builders.Person;
using Domain.Entity.Person;
using Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Person.Handlers
{
    public class CreateEmployeeHandler : IRequestHandler<CreateEmployeeCommand, EmployeeDTO>
    {
        private readonly IUnitOfWork _uow;

        public CreateEmployeeHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<EmployeeDTO> Handle(CreateEmployeeCommand request, CancellationToken ct)
        {

            var company = await _uow.Companies.GetByIdAsync(request.CompanyId);
            if (company == null) throw new Exception("Company not found, big boss!");

            var employeeBuilder = new EmployeeBuilder()
                .WithName(request.Name)
                .WithEmail(new EmailAddress(request.Email))
                .WithEmployeeType((EmployeeType)request.EmployeeType)
                .WithAutonomy(request.IsAutonomous);

            var employee = company.CreateEmployee(employeeBuilder);

            await _uow.Employees.AddAsync(employee);
            await _uow.CommitTransactionAsync();

            return EmployeeDTO.FromEntity(employee);
        }
    }
}
