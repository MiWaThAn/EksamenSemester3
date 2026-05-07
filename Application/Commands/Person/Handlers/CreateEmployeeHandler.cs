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
            // 1. Get the boss (The Company)
            var company = await _uow.Companies.GetByIdAsync(request.CompanyId);
            if (company == null) throw new Exception("Company not found, big boss!");

            // 2. Prepare the Employee Builder (Your Domain logic)
            var employeeBuilder = new EmployeeBuilder()
                .WithName(request.Name)
                .WithEmail(new EmailAddress(request.Email))
                .WithEmployeeType((EmployeeType)request.EmployeeType)
                .WithAutonomy(request.IsAutonomous);

            // 3. The Magic Move: The Company creates the employee internally
            var employee = company.CreateEmployee(employeeBuilder);

            // 4. Persist the changes
            // Because the employee was added to company._employees, 
            // the UoW/EF Core tracks this new child object.
            await _uow.Employees.AddAsync(employee); // Some setups need this, others do it via Company
            await _uow.CommitTransactionAsync();

            return EmployeeDTO.FromEntity(employee);
        }
    }
}
