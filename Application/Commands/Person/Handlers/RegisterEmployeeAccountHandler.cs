using Application.Commands.Person.Responses;
using Application.DataSeeding.Auth;
using Application.Interfaces;
using Domain.Guards;
using Domain.Interfaces.Person;
using Domain.ValueObjects;
using MediatR;
using Shared.Person.Auth.Commands;
using Shared.Person.Auth.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Person.Handlers
{
    public class RegisterEmployeeAccountHandler : IRequestHandler<RegisterEmployeeAccountCommand, RegisterEmployeeAccountResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRegistrationDomainService _registrationDomainService;
        public RegisterEmployeeAccountHandler(IUnitOfWork unitOfWork, IRegistrationDomainService registrationDomainService)
        {
            _unitOfWork = unitOfWork;
            _registrationDomainService = registrationDomainService;
        }
        public async Task<RegisterEmployeeAccountResponse> Handle(RegisterEmployeeAccountCommand request, CancellationToken ct)
        {
            Guard.AgainstEmptyGuid(request.EmployeeId, nameof(request.EmployeeId));
            await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted,ct);
            var employee = await _unitOfWork.Employees.GetByIdAsync(request.EmployeeId,ct);
            Guard.AgainstNull(employee,nameof(employee));
            PhoneNumber phoneNumber = new(request.PhoneNumber);
            var result = await _registrationDomainService.RegisterEmployeeAccountAsync(phoneNumber, request.Username, request.Password, employee,ct);
            if (result.IsFailure)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new RegisterEmployeeAccountResponse
                {
                    Success = false,
                    Message = result.Error
                };
            }
            var account = result.Value;
            var empRole = await _unitOfWork.Roles.GetByTitleAsync(SystemRoles.Employee) ?? throw new InvalidOperationException("System Rolle 'Employee' not found. Ensure DataSeeder has run.");
            account.AddRole(empRole);
            await _unitOfWork.Accounts.AddAsync(account);
            await _unitOfWork.CompleteAsync(ct);
            await _unitOfWork.CommitTransactionAsync();
            return new RegisterEmployeeAccountResponse
            {
                Success = true,
                Id = account.Id
            };
        }
    }
}
