using Application.Commands.Person.Responses;
using Application.DataSeeding.Auth;
using Application.Interfaces;
using Domain.Builders.Person;
using Domain.Entity.Person;
using Domain.Interfaces.Person;
using Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Shared.Person.Auth.Commands;
using Shared.Person.Auth.Responses;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace Application.Commands.Person.Handlers.Auth.Registration
{
    public class RegisterCompanyHandler(IUnitOfWork _unitOfWork, IRegistrationDomainService _registrationDomainService) : IRequestHandler<RegisterCompanyCommand, RegisterCompanyResponse>
    {
        public async Task<RegisterCompanyResponse> Handle(RegisterCompanyCommand request, CancellationToken ct)
        {
            bool Active = false;
            CvrNumber cvr = new(request.CVRNumber);
            EmailAddress email = new(request.Email);
            PhoneNumber phoneNumber = new(request.PhoneNumber);
            try
            {
                await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, ct);
                Active = true;
                var result = await _registrationDomainService.RegisterCompanyAccountAsync(request.CompanyName, cvr, email, phoneNumber, request.Username, request.Password,ct);
                if (result.IsFailure)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new RegisterCompanyResponse
                    {
                        Success = false,
                        Message = result.Error
                    };
                }
                var (company, account) = result.Value;
                var companyRole = await _unitOfWork.Roles.GetByTitleAsync(SystemRoles.Company) ?? throw new InvalidOperationException("System Role 'Company' not found. Ensure DataSeeder has run.");
                account.AddRole(companyRole);
                await _unitOfWork.Companies.AddAsync(company);
                await _unitOfWork.Accounts.AddAsync(account);
                await _unitOfWork.CompleteAsync(ct);
                await _unitOfWork.CommitTransactionAsync(ct);
                return new RegisterCompanyResponse
                {
                    Success = true,
                    Id = account.Id
                };
            }
            catch (Exception)
            {
                if (Active)
                    await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
