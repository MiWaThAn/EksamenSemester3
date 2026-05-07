using Domain.Entity.Person;
using Domain.Guards;
using Domain.Interfaces.Person;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Builders.Person
{
    public class CompanyBuilder
    {
        internal CvrNumber CVRNumber;
        internal Account Account;
        internal EmailAddress Email;
        internal string Name;
        public CompanyBuilder WithCVRNumber(CvrNumber cvrNumber)
        {
            CVRNumber = cvrNumber;
            return this;
        }

        public CompanyBuilder WithName(string name)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Name = name;
            return this;
        }
        public CompanyBuilder WithEmail(EmailAddress email)
        {
            Email = email;
            return this;
        }
        //Company can only be built through an account (to make sure every company has an account) so we don't end up with accountless companies
        internal CompanyBuilder WithAccount(Account account)
        {
            Account = account;
            return this;
        }
        internal Company Build()
        {
            Guard.AgainstNullOrEmpty(Name, nameof(Name));
            Guard.AgainstNull(Account,nameof(Account));
            return new Company(Name, CVRNumber, Account, Email);
        }
    }
}
