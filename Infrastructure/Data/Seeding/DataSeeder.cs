using Application.DataSeeding.Auth;
using Application.Interfaces;
using Domain.Builders.Person;
using Domain.Entity.Person.Auth;
using Domain.Interfaces.Item;
using Domain.Interfaces.Person;
using Domain.Services.Person;
using Domain.ValueObjects;
using Infrastructure.Service.Security;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Data.Seeding
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(AppDbContext context,IRegistrationDomainService registration,IHashingService hashing)
        {
            await SeedPermsAndRoles(context);
            await SeedCompanyTestAccount(context,hashing,registration);
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
                var perms = await context.Permissions.Where(p => p.Title == SystemPermissions.ProjectWrite || p.Title == SystemPermissions.ProjectRead).ToListAsync();
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
        private static async Task SeedCompanyTestAccount(AppDbContext context,IHashingService hashingService,IRegistrationDomainService registrationDomainService)
        {
            if (!context.Companies.Any(c => c.Name == "Admin" && c.Account.Username == "admin"))
            {
                var result = await registrationDomainService.RegisterCompanyAccountAsync("Admin", new CvrNumber("12345678"), new EmailAddress("FASTApp@Gmail.com"), new PhoneNumber("12345678"), "admin", "admin");
                if (result.IsSuccess)
                {
                    var (company, account) = result.Value;
                    var companyRole = await context.Roles.FirstOrDefaultAsync(r => r.Title == SystemRoles.Company) ?? throw new InvalidOperationException("System Role 'Company' not found. Ensure DataSeeder has run.");
                    account.AddRole(companyRole);
                    await context.Accounts.AddAsync(account);
                    await context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception($"Seeding Failed! Domain Error: {result.Error}");
                }
            }
        }
    }
}
