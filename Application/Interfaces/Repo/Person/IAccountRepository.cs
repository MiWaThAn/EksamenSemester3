using Domain.Entity.Person;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Repo.Person
{
    public interface IAccountRepository<T> : IGenericRepository<T> where T : class
    {

    }
}