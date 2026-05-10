using Application.Commands.Person.Responses;
using Application.Interfaces;
using Domain.Guards;
using Domain.Interfaces.Person;
using Domain.ValueObjects;
using MediatR;
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
            await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            var employee = await _unitOfWork.Employees.GetByIdAsync(request.EmployeeId);
            Guard.AgainstNull(employee,nameof(employee));
            PhoneNumber phoneNumber = new(request.PhoneNumber);
            var result = await _registrationDomainService.RegisterEmployeeAccountAsync(phoneNumber, request.Username, request.Password, employee);
            if (result.IsFailure)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new RegisterEmployeeAccountResponse
                {
                    Success = false
                };
            }
            var account = result.Value;
            await _unitOfWork.Accounts.AddAsync(account);
            await _unitOfWork.CompleteAsync();
            await _unitOfWork.CommitTransactionAsync();
            return new RegisterEmployeeAccountResponse
            {
                Success = true,
                Id = account.Id
            };
        }
    }
}
