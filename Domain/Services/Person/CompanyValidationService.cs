using Domain.Interfaces.Person;
using Domain.Interfaces.Repos;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Services.Person
{
    public class CompanyValidationService : ICompanyValidationService
    {
        private readonly ICompanyRepository _companyRepository;
        public CompanyValidationService(ICompanyRepository companyRepository) 
        {
            _companyRepository = companyRepository;
        }
        public async Task<bool> CvrExistsAsync(CvrNumber cvrNumber, CancellationToken ct = default)
        {
            var existingCompany = await _companyRepository.GetByCVRAsync(cvrNumber,ct);
            return existingCompany == null;
        }

    }
}
