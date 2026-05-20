using Application.Interfaces.Repo.Person.Auth;
using Domain.Entity.Person.Auth;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories.Person.Auth
{
    internal class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(AppDbContext context):base(context) { }
        public async Task<Role?> GetByTitleAsync(string title)
        {
            return _context.Roles.FirstOrDefault(r => r.Title == title);
        }
    }
}
