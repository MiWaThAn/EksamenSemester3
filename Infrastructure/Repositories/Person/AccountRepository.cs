using Application.Interfaces.Repo.Person;
using Domain.Interfaces.Repos;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories.Person
{
    internal class AccountRepository<T> : GenericRepository<T>, IAccountRepository<T> where T : class
    {
        internal AccountRepository(AppDbContext context) : base(context) { }
    }
}
