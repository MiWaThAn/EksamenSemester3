using Application.Commands.Person.Queries;
using Application.DTOs;
using Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Person.Handlers
{
    public class GetEmployeeByIdHandler : IRequestHandler<GetEmployeeByIdQuery, EmployeeDTO>
    {
        private readonly IUnitOfWork _uow;

        public GetEmployeeByIdHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<EmployeeDTO> Handle(GetEmployeeByIdQuery request, CancellationToken ct)
        {
            // Use your Repo to find the employee lore
            var employee = await _uow.Employees.GetByIdAsync(request.Id);

            if (employee == null) return null;

            return EmployeeDTO.FromEntity(employee);
        }
    }
}
