using Domain.Builders.Person;
using Domain.Entity.Person;
using Domain.Guards;
using Domain.Interfaces.Person;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Services.Person
{
    public class AccountFactory : IAccountFactory
    {
        private IAccountValidationService _validationService;
        internal AccountFactory(IAccountValidationService validationService)
        {
            _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
        }
        public async Task<Result<Account>> CreateAsync(AccountBuilder builder)
        {
            Guard.AgainstNull(builder, nameof(builder));
            if(await _validationService.UsernameExistsAsync(builder.Username))return Result<Account>.Failure("Brugernavnet er allerede i brug.");
            return Result<Account>.Success(builder.Build());
        }
    }
}

