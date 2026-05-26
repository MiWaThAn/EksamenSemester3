using Application.DataSeeding.Auth;
using Application.Interfaces;
using Domain.Builders.Item;
using Domain.Builders.Person;
using Domain.Entity.Item;
using Domain.Entity.Mapping;
using Domain.Entity.Mapping.ValueObjects;
using Domain.Entity.Person;
using Domain.Entity.Person.Auth;
using Domain.Interfaces.Item;
using Domain.Interfaces.Mapping;
using Domain.Interfaces.Person;
using Domain.Services.Mapping;
using Domain.Services.Person;
using Domain.ValueObjects;
using Infrastructure.Service.Security;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Infrastructure.Data.Seeding
{
    public static class DataSeeder
    {
        

        public static async Task SeedAsync(AppDbContext context,IRegistrationDomainService registration,IHashingService hashing, IProviderFactory providerFactory)
        {
            await SeedPermsAndRoles(context);
            await SeedCompanyTestAccount(context,hashing,registration);
            await SeedProviders(context, providerFactory);
            await SeedActivities(context);
            await SeedProjects(context);
            await SeedProjectActivities(context);
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
        private static async Task SeedCompanyTestAccount(AppDbContext context, IHashingService hashingService, IRegistrationDomainService registrationDomainService)
        {
            if (!context.Companies.Any(c => c.Name == "Admin" && c.Account.Username == "admin"))
            {
                var result = await registrationDomainService.RegisterCompanyAccountAsync("Admin", new CvrNumber("12345678"), new EmailAddress("FASTApp@Gmail.com"), new PhoneNumber("12345678"), "admin", "admin");
                if (result.IsSuccess)
                {
                    var (company, account) = result.Value;


                    var companyRole = await context.Roles.FirstOrDefaultAsync(r => r.Title == SystemRoles.Company)
                        ?? throw new InvalidOperationException("System Role 'Company' not found.");
                    var employeeRole = await context.Roles.FirstOrDefaultAsync(r => r.Title == SystemRoles.Employee)
                        ?? throw new InvalidOperationException("System Role 'Employee' not found.");
                    var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Title == SystemRoles.Admin)
                        ?? throw new InvalidOperationException("System Role 'Admin' not found.");


                    account.AddRole(companyRole);
                    account.AddRole(employeeRole);
                    account.AddRole(adminRole);


                    var employeeBuilder = new EmployeeBuilder()
                        .WithName("Søren Hansen")
                        .WithEmail(new EmailAddress("soren@byggefirma.dk"))
                        .WithAutonomy(true)
                        .WithEmployeeType(EmployeeType.Formand);


                    var employee = company.CreateEmployee(employeeBuilder);


                    employee.LinkToAccount(account);


                    account.LinkToEmployee(employee);

                    await context.Accounts.AddAsync(account);
                    await context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception($"Seeding Failed! Domain Error: {result.Error}");
                }
            }
        }
        private static async Task SeedProviders(AppDbContext context, IProviderFactory providerFactory)
        {
            if (await context.Providers.AnyAsync()) return;

            var result = await providerFactory.CreateAsync(
                DataSource.From("economic"),
                new Dictionary<IntegrationEntityType, string>
                {
            { IntegrationEntityType.From("customer", 1), "https://apis.e-conomic.com/customersapi/v3.1.0/Customers" },
            { IntegrationEntityType.From("employee", 2), "https://apis.e-conomic.com/projectsapi/v1.1.0/Employees" },
            { IntegrationEntityType.From("project", 3), "https://apis.e-conomic.com/projectsapi/v1.1.0/Projects" },
                });

            if (result.IsFailure)
                throw new Exception($"Provider seeding failed: {result.Error}");

            context.Providers.Add(result.Value);
            await context.SaveChangesAsync();
        }
        private static async Task SeedProjects(AppDbContext context)
        {
            if (await context.Projects.AnyAsync())
                return;
            var company = await context.Companies
            .Include(c => c.Projects)
            .Include(c => c.Activities)
            .Include(c => c.Employees)
            .FirstOrDefaultAsync(c => c.Name == "Admin" && c.Account.Username == "admin");
            if (!await context.Projects.AnyAsync(p => p.CompanyId == company.Id))
            {
                var employee = company.Employees.FirstOrDefault();

                var projectBuilder1 = new ProjectBuilder()
                    .WithName("Søndergade Renovering")
                    .WithDescription("Hovedentreprise på renovering af lejlighedskompleks")
                    .WithIsStatus(Status.Åben);

                var projectBuilder2 = new ProjectBuilder()
                    .WithName("Nyt Typehus - Aarhus")
                    .WithDescription("Opførelse af 180kvm parcelhus")
                    .WithIsStatus(Status.Åben);

                var project1 = company.CreateProject(projectBuilder1);
                var project2 = company.CreateProject(projectBuilder2);

                if (employee != null)
                {
                    project1.LinkToEmployee(employee);
                    project2.LinkToEmployee(employee);
                    project1.AssignEmployee(employee);
                    project2.AssignEmployee(employee);
                }

                await context.Projects.AddRangeAsync(project1, project2);
                await context.SaveChangesAsync();
            }
        }
        private static async Task SeedProjectActivities(AppDbContext context)
        {
            if (await context.ProjectActivities.AnyAsync())
                return;
            var company = await context.Companies
            .Include(c => c.Projects)
            .Include(c => c.Activities)
            .Include(c => c.Employees)
            .FirstOrDefaultAsync(c => c.Name == "Admin" && c.Account.Username == "admin");
            var targetProject = await context.Projects
                .Include(p => p.Activities)
                .FirstOrDefaultAsync(p => p.Name == "Søndergade Renovering" && p.CompanyId == company.Id);

            if (targetProject != null && !targetProject.Activities.Any())
            {
                var carpenterActivity = await context.Activities.FirstOrDefaultAsync(a => a.Name == "Tømrerarbejde" && a.CompanyId == company.Id);
                var transportActivity = await context.Activities.FirstOrDefaultAsync(a => a.Name == "Kørsel & Transport" && a.CompanyId == company.Id);
                var employee = company.Employees.FirstOrDefault();

                if (carpenterActivity != null)
                {
                    var paBuilder1 = new ProjectActivityBuilder()
                        .WithActivity(carpenterActivity)
                        .WithStartAndEndDates(DateTime.UtcNow, DateTime.UtcNow.AddMonths(3))
                        .WithStatus(Status.Åben);

                    if (employee != null) paBuilder1.WithResponsibleEmployee(employee);

                    var projact = targetProject.CreateProjectActivity(paBuilder1);
                    await context.ProjectActivities.AddAsync(projact);
                }

                if (transportActivity != null)
                {
                    var paBuilder2 = new ProjectActivityBuilder()
                        .WithActivity(transportActivity)
                        .WithStartAndEndDates(DateTime.UtcNow, DateTime.UtcNow.AddMonths(1))
                        .WithStatus(Status.Åben);

                    var projact = targetProject.CreateProjectActivity(paBuilder2);
                    await context.ProjectActivities.AddAsync(projact);
                }

                await context.SaveChangesAsync();
            }
        }
        private static async Task SeedActivities(AppDbContext context)
        {
            if (await context.Activities.AnyAsync())
                return;
            var company = await context.Companies
                .Include(c => c.Projects)
                .Include(c => c.Activities)
                .Include(c => c.Employees)
                .FirstOrDefaultAsync(c => c.Name == "Admin" && c.Account.Username == "admin");

            if (company == null) return;

            if (!await context.Activities.AnyAsync(a => a.CompanyId == company.Id))
            {
                var activityBuilder1 = new ActivityBuilder().WithName("Tømrerarbejde").WithDescription("Standard tømrer og snedker opgaver");
                var activityBuilder2 = new ActivityBuilder().WithName("Murerarbejde").WithDescription("Fliselægning og opmuring");
                var activityBuilder3 = new ActivityBuilder().WithName("Kørsel & Transport").WithDescription("Transporttid til og fra byggeplads");

                var act1 = company.CreateActivity(activityBuilder1);
                var act2 = company.CreateActivity(activityBuilder2);
                var act3 = company.CreateActivity(activityBuilder3);

                await context.Activities.AddRangeAsync(act1, act2, act3);
                await context.SaveChangesAsync();
            }
        }

    }
}
