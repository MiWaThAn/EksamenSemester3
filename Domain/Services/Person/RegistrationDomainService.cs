using Domain.Builders.Person;
using Domain.Entity.Person;
using Domain.Interfaces.Item;
using Domain.Interfaces.Person;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Services.Person
{
    public class RegistrationDomainService : IRegistrationDomainService
    {
        private readonly IHashingService _passwordHasher;
        private readonly ICompanyValidationService _companyValidationService;
        private readonly IAccountValidationService _accountValidationService;
        private readonly IAccountFactory _accountFactory;
        private readonly ICompanyFactory _companyFactory;
        public RegistrationDomainService(IHashingService passwordHasher, IAccountValidationService accountValidationService, ICompanyValidationService companyValidationService, IAccountFactory accountFactory, ICompanyFactory companyFactory)
        {
            _passwordHasher = passwordHasher;
            _accountValidationService = accountValidationService;
            _companyValidationService = companyValidationService;
            _accountFactory = accountFactory;
            _companyFactory = companyFactory;
        }

        public async Task<Result<(Company, Account)>> RegisterCompanyAccountAsync(string companyName, CvrNumber cvrNumber, EmailAddress emailAddress, PhoneNumber phoneNumber, string username, string plainTextPassword, CancellationToken ct = default)
        {
            if (await _accountValidationService.UsernameExistsAsync(username,ct)) return Result<(Company, Account)>.Failure("Brugernavn er allerede i brug");
            //Service is nessary since we need unique cvr numbers to login accounts.
            if (await _companyValidationService.CvrExistsAsync(cvrNumber,ct)) return Result<(Company, Account)>.Failure("Cvr er allerede i brug.");
            //Encryption service uses master key from app settings and needs a Dcrypt method to be able to use the encrypted tokens when needed. The password hasher is used to hash the password before saving it to the database, and it also needs a VerifyHashedPassword method to verify the password when logging in.
            // string encryptedGrantToken = _encryptionService.Encrypt(request.AgreementGrantToken); <-- We need to have a seperate intergrate economic command that encrypts the token and store it in the company integration settings.
            string securePassHash = _passwordHasher.Hash(plainTextPassword);
            var companyBuilder = new CompanyBuilder()
                .WithCVRNumber(cvrNumber)
                .WithEmail(emailAddress)
                .WithName(companyName);
            var accountBuilder = new AccountBuilder()
                .WithHashedPassword(securePassHash)
                .WithPhoneNumber(phoneNumber)
                .WithUsername(username);
            var result = await _accountFactory.CreateAsync(accountBuilder,ct);
            if (result.IsFailure)
            {
                return Result<(Company, Account)>.Failure(result.Error);
            }
            var account = result.Value;
            var resultCompany = await account.CreateCompany(companyBuilder, _companyFactory,ct);
            if (resultCompany.IsFailure)
            {
                return Result<(Company, Account)>.Failure(result.Error);
            }
            var company = resultCompany.Value;
            return Result<(Company, Account)>.Success((company, account));
        }
        public async Task<Result<Account>> RegisterEmployeeAccountAsync(PhoneNumber phoneNumber, string username, string plainTextPassword, Employee employee, CancellationToken ct = default)
        {
            if (await _accountValidationService.UsernameExistsAsync(username,ct)) 
                return Result<Account>.Failure("Brugernavn er allerede i brug");
            string securePassHash = _passwordHasher.Hash(plainTextPassword);
            var accountBuilder = new AccountBuilder()
                .WithHashedPassword(securePassHash)
                .WithPhoneNumber(phoneNumber)
                .WithUsername(username)
                .WithEmployee(employee);
            var result = await _accountFactory.CreateAsync(accountBuilder,ct);
            if (result.IsFailure)
            {
                return Result<Account>.Failure(result.Error);
            }
            var account = result.Value;
            employee.LinkToAccount(account);
            return Result<Account>.Success(account);
        }
    }
}
