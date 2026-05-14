using Domain.Entity.Person;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services
{
    public interface ITokenService
    {
        string GetToken(Account account);
    }
}
