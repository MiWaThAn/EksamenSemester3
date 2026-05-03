using Application.Commands.Person.Responses;
using Application.Interfaces;
using Domain.Builders.Person;
using Domain.Entity.Person;
using Microsoft.AspNetCore.Identity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Person.Handlers
{
    public class RegisterCompanyHandler : IRequestHandler<RegisterCompanyCommand, RegisterCompanyResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<Account> _passwordHasher;
        public RegisterCompanyHandler(IUnitOfWork unitOfWork, IPasswordHasher<Account> passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task<RegisterCompanyResponse> Handle(RegisterCompanyCommand request, CancellationToken ct)
        {
            //Service is nessary since we need unique cvr numbers to login accounts.
            if (await _validationService.CvrExists(request.CVRNumber))
            {
                return new RegisterCompanyResponse
                {
                    Success = false,
                    Message = "CVR nummeret eksisterer allerede"
                };
            }
            //Encryption service uses master key from app settings and needs a Dcrypt method to be able to use the encrypted tokens when needed. The password hasher is used to hash the password before saving it to the database, and it also needs a VerifyHashedPassword method to verify the password when logging in.
            // string encryptedGrantToken = _encryptionService.Encrypt(request.AgreementGrantToken); <-- We need to have a seperate intergrate economic command that encrypts the token and store it in the company integration settings.
            string secureHash = _passwordHasher.HashPassword(default!, request.Password);

            var company = new CompanyBuilder()
                .WithCVRNumber(request.CVRNumber)
                .WithEmail(request.Email)
                .WithName(request.Name)
                .WithHashedPassword(secureHash)
                .WithPhoneNumber(request.PhoneNumber)
                .WithUsername(request.Username)
                .Build();
            await _unitOfWork.Companies.AddAsync(company);
            await _unitOfWork.CompleteAsync();

            return new RegisterCompanyResponse
            {
                Success = true,
                Id = company.Id
            };
        }
    }
}
