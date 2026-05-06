using Domain.Builders.Person;
using Domain.Entity.Person;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces.Person
{
    public interface IAccountFactory
    {
        Task<Result<Account>> CreateAsync(AccountBuilder builder);
    }
}
