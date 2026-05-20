using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces.Person
{
    public interface IAccountValidationService
    {
        Task<bool> UsernameExistsAsync(string username, CancellationToken ct = default);
    }
}
