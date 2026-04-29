using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Builders.Person
{
    public abstract class AccountBuilder<TBuilder, TEntity> where TBuilder : AccountBuilder<TBuilder, TEntity>
    {
        protected string Name;
        protected string HashedPassword;
        protected string Username;
        protected string Email;
        protected string PhoneNumber;
        public TBuilder WithName(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            return (TBuilder)this;
        }
        public TBuilder WithHashedPassword(string hashedPassword)
        {
            HashedPassword = hashedPassword ?? throw new ArgumentNullException(nameof(hashedPassword));
            return (TBuilder)this;
        }
        public TBuilder WithUsername(string username)
        {
            Username = username ?? throw new ArgumentNullException(nameof(username));
            return (TBuilder)this;
        }
        public TBuilder WithEmail(string email)
        {
            Email = email ?? throw new ArgumentNullException(nameof(email));
            return (TBuilder)this;
        }
        public TBuilder WithPhoneNumber(string phoneNumber)
        {
            PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
            return (TBuilder)this;
        }
    }
}
