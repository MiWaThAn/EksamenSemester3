using Domain.Builders.Person;
using Domain.Entity.Person;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces.Person
{
    public interface ICompanyFactory
    {
        Task<Result<Company>> CreateAsync(CompanyBuilder builder, Account account);
    }
}
