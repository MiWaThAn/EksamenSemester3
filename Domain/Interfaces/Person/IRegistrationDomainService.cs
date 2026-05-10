using Domain.Builders.Person;
using Domain.Entity.Person;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces.Person
{
    public interface IRegistrationDomainService
    {
        Task<Result<(Company,Account)>> RegisterCompanyAccountAsync(
            string companyName,
            CvrNumber cvrNumber,
            EmailAddress emailAddress,
            PhoneNumber phoneNumber,
            string username,
            string plainTextPassword
            );
        Task<Result<Account>> RegisterEmployeeAccountAsync(
            PhoneNumber phoneNumber,
            string username,
            string plainTextPassword,
            Employee employee);
    }
}
