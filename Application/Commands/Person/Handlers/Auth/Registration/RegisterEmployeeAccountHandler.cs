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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Application.Commands.Person.Handlers.Auth.Registration
{
    public class RegisterEmployeeAccountHandler(IUnitOfWork _unitOfWork, IRegistrationDomainService _registrationDomainService) : IRequestHandler<RegisterEmployeeAccountCommand, RegisterEmployeeAccountResponse>
    {
        public async Task<RegisterEmployeeAccountResponse> Handle(RegisterEmployeeAccountCommand request, CancellationToken ct)
        {
            bool Active = false;
            Guard.AgainstNullOrEmpty(request.EmployeeId, nameof(request.EmployeeId));
            if (!Guid.TryParse(request.EmployeeId, out var EmployeeId))
            {
                return new RegisterEmployeeAccountResponse { Success = false, Message = "Ugyldigt konto-ID format." };
            }
            try
            {
                await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, ct);
                Active = true;
                var employee = await _unitOfWork.Employees.GetByIdAsync(EmployeeId, ct);
                Guard.AgainstNull(employee, nameof(employee));
                PhoneNumber phoneNumber = new(request.PhoneNumber);
                var result = await _registrationDomainService.RegisterEmployeeAccountAsync(phoneNumber, request.Username, request.Password, employee, ct);
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
                await _unitOfWork.CommitTransactionAsync(ct);
                return new RegisterEmployeeAccountResponse
                {
                    Success = true,
                    Id = account.Id
                };
            }
            catch (Exception)
            {
                if(Active)
                    await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
