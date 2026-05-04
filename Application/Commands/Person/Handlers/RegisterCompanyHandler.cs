using Application.Commands.Person.Responses;
using Application.Interfaces;
using Domain.Builders.Person;
using Domain.Entity.Person;
using Microsoft.AspNetCore.Identity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Interfaces.Person;
using Domain.ValueObjects;

namespace Application.Commands.Person.Handlers
{
    public class RegisterCompanyHandler : IRequestHandler<RegisterCompanyCommand, RegisterCompanyResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<Account> _passwordHasher;
        private readonly ICompanyValidationService _companyValidationService;
        private readonly IAccountValidationService _accountValidationService;
        private readonly IAccountFactory _accountFactory;
        private readonly ICompanyFactory _companyFactory;
        public RegisterCompanyHandler(IUnitOfWork unitOfWork, IPasswordHasher<Account> passwordHasher, IAccountValidationService accountValidationService,ICompanyValidationService companyValidationService,IAccountFactory accountFactory,ICompanyFactory companyFactory)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _accountValidationService = accountValidationService;
            _companyValidationService = companyValidationService;
            _accountFactory = accountFactory;
            _companyFactory = companyFactory;
        }

        public async Task<RegisterCompanyResponse> Handle(RegisterCompanyCommand request, CancellationToken ct)
        {
            await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            CvrNumber cvr = new CvrNumber(request.CVRNumber);
            //Service is nessary since we need unique cvr numbers to login accounts.
            if (await _companyValidationService.CvrExistsAsync(cvr))
            {
                await _unitOfWork.RollbackTransactionAsync();
                _unitOfWork.Dispose();
                return new RegisterCompanyResponse
                {
                    Success = false,
                    Message = "CVR nummeret eksisterer allerede"
                };
            }
            //Encryption service uses master key from app settings and needs a Dcrypt method to be able to use the encrypted tokens when needed. The password hasher is used to hash the password before saving it to the database, and it also needs a VerifyHashedPassword method to verify the password when logging in.
            // string encryptedGrantToken = _encryptionService.Encrypt(request.AgreementGrantToken); <-- We need to have a seperate intergrate economic command that encrypts the token and store it in the company integration settings.
            string secureHash = _passwordHasher.HashPassword(default!, request.Password);
            string pinHash = _passwordHasher.HashPassword(default!, request.Pincode);
            EmailAddress emailAddress = new EmailAddress(request.Email);

            var companyBuilder = new CompanyBuilder()
                .WithCVRNumber(cvr)
                .WithEmail(emailAddress)
                .WithName(request.Name);

            var accountBuilder = new AccountBuilder()
                .WithHashedPassword(secureHash)
                .WithHashedPin(pinHash)
                .WithPhoneNumber(request.PhoneNumber)
                .WithUsername(request.Username);

            var result = await _accountFactory.CreateAsync(accountBuilder);

            if(result.IsFailure)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _unitOfWork.Dispose();
                return new RegisterCompanyResponse
                {
                    Success = false,
                    Message = "Firmaet kunne ikke laves"
                };
            }
            var account = result.Value;
            var resultCompany = await account.CreateCompany(companyBuilder, _companyFactory);
            if(resultCompany.IsFailure)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _unitOfWork.Dispose();
                return new RegisterCompanyResponse
                {
                    Success = false,
                    Message = "Firmaet kunne ikke laves"
                };
            }
            await _unitOfWork.Companies.AddAsync(resultCompany.Value);
            await _unitOfWork.Accounts.AddAsync(account);
            await _unitOfWork.CommitTransactionAsync();

            return new RegisterCompanyResponse
            {
                Success = true,
                Id = account.Id
            };
        }
    }
}
