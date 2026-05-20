using Application.Interfaces.Repo.Person.Auth;
using Domain.Entity.Person.Auth;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories.Person.Auth
{
    internal class PermissionRepository : GenericRepository<Permission>, IPermissionRepository
    {
        public PermissionRepository(AppDbContext dbContext) : base(dbContext) { }
    }
}
