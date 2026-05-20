using Application.DataSeeding.Auth;
using Domain.Entity.Person.Auth;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Data.Seeding
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            await SeedPermsAndRoles(context);
        }
        private static async Task SeedPermsAndRoles(AppDbContext context)
        {
            var allPerms = new List<string> {
            SystemPermissions.ProjectRead,
            SystemPermissions.ProjectWrite,
            SystemPermissions.UserManage
        };

            foreach (var p in allPerms)
            {
                if (!await context.Permissions.AnyAsync(x => x.Title == p))
                    context.Permissions.Add(new Permission(p));
            }
            await context.SaveChangesAsync();

            var adminRole = await context.Roles
                .Include(r => r.Permissions)
                .FirstOrDefaultAsync(r => r.Title == SystemRoles.Admin);

            if (adminRole == null)
            {
                adminRole = new Role(SystemRoles.Admin);
                var perms = await context.Permissions.ToListAsync();
                foreach (var p in perms) adminRole.AddPermissions(p);

                context.Roles.Add(adminRole);
            }
            var empRole = await context.Roles.Include(r => r.Permissions).FirstOrDefaultAsync(r => r.Title == SystemRoles.Employee);

            if (empRole == null)
            {
                empRole = new Role(SystemRoles.Employee);
                var perms = await context.Permissions.Where(p=>p.Title == SystemPermissions.ProjectWrite || p.Title == SystemPermissions.ProjectRead).ToListAsync();
                foreach (var p in perms) empRole.AddPermissions(p);

                context.Roles.Add(empRole);
            }
            var companyRole = await context.Roles.Include(r => r.Permissions).FirstOrDefaultAsync(r => r.Title == SystemRoles.Company);

            if (companyRole == null)
            {
                companyRole = new Role(SystemRoles.Company);
                var perms = await context.Permissions.ToListAsync();
                foreach (var p in perms) companyRole.AddPermissions(p);

                context.Roles.Add(companyRole);
            }

            await context.SaveChangesAsync();
        }
    }
}
