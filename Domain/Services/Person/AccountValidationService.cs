using Domain.Interfaces.Person;
using Domain.Interfaces.Repos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Services.Person
{
    public class AccountValidationService : IAccountValidationService
    {
        private readonly IAccountRepository _accountRepository;
        public AccountValidationService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        public async Task<bool> UsernameExistsAsync(string username)
        {
            if (string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username));
            var account = await _accountRepository.GetByUsernameAsync(username);
            return account == null;
        }
    }
}
