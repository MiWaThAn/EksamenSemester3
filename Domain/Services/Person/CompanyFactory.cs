using Domain.Builders.Person;
using Domain.Entity.Person;
using Domain.Interfaces.Person;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Services.Person
{
    public class CompanyFactory : ICompanyFactory
    {
        private ICompanyValidationService _validationService;
        internal CompanyFactory(ICompanyValidationService validationService)
        {
            _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
        }
        public async Task<Result<Company>> CreateAsync(CompanyBuilder builder, Account account, CancellationToken ct = default)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if(await _validationService.CvrExistsAsync(builder.CVRNumber,ct)) return Result<Company>.Failure($"Et firma med dette CVR nummer {builder.CVRNumber} findes alerede.");
            return Result<Company>.Success(builder.WithAccount(account).Build());
        }
    }
}
