using Domain.Entity.Person;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Builders.Person
{
    public class CompanyBuilder : AccountBuilder<CompanyBuilder, Company>
    {
        private string CVRNumber;
        private string HashedAgreementGrantToken;
        private string HashedAppSecretToken;
        private string HashedEconomicAgreementNumber;
        public CompanyBuilder WithCVRNumber(string cvrNumber)
        {
            CVRNumber = cvrNumber ?? throw new ArgumentNullException(nameof(cvrNumber));
            return this;
        }
        public CompanyBuilder WithHashedAgreementGrantToken(string hashedAgreementGrantToken)
        {
            HashedAgreementGrantToken = hashedAgreementGrantToken ?? throw new ArgumentNullException(nameof(hashedAgreementGrantToken));
            return this;
        }
        public CompanyBuilder WithHashedAppSecretToken(string hashedAppSecretToken)
        {
            HashedAppSecretToken = hashedAppSecretToken ?? throw new ArgumentNullException(nameof(hashedAppSecretToken));
            return this;
        }
        public CompanyBuilder WithHashedEconomicAgreementNumber(string hashedEconomicAgreementNumber)
        {
            HashedEconomicAgreementNumber = hashedEconomicAgreementNumber ?? throw new ArgumentNullException(nameof(hashedEconomicAgreementNumber));
            return this;
        }
        public Company Build()
        {
            if (string.IsNullOrEmpty(Name)) throw new InvalidOperationException("Name is required to build a company.");
            if (string.IsNullOrEmpty(HashedPassword)) throw new InvalidOperationException("Hashed password is required to build a company.");
            if (string.IsNullOrEmpty(Username)) throw new InvalidOperationException("Username is required to build a company.");
            if (string.IsNullOrEmpty(CVRNumber)) throw new InvalidOperationException("CVR number is required to build a company.");
            if (string.IsNullOrEmpty(HashedAgreementGrantToken)) throw new InvalidOperationException("Hashed agreement grant token is required to build a company.");
            if (string.IsNullOrEmpty(HashedAppSecretToken)) throw new InvalidOperationException("Hashed app secret token is required to build a company.");
            if (string.IsNullOrEmpty(HashedEconomicAgreementNumber)) throw new InvalidOperationException("Hashed economic agreement number is required to build a company.");
            return new Company(Name, HashedPassword, Username, Email, PhoneNumber, CVRNumber, HashedAgreementGrantToken, HashedAppSecretToken, HashedEconomicAgreementNumber);
        }
    }
}
