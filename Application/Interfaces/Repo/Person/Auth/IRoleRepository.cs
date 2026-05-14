using Domain.Entity.Person.Auth;
using Domain.Interfaces.Repos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Repo.Person.Auth
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        public Task<Role?> GetByTitleAsync(string title);
    }
}
