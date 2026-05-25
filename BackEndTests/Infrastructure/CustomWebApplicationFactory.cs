using API;
using API.Workers;
using Domain.Entity.Item;
using Domain.Entity.Person;
using Domain.Entity.Person.Auth;
using Domain.ValueObjects;
using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private SqliteConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var descriptors = services.Where(d =>
                d.ServiceType.Name.Contains("DbContextOptions") ||
                d.ServiceType == typeof(AppDbContext)).ToList();

            foreach (var descriptor in descriptors) services.Remove(descriptor);

            _connection = new SqliteConnection("DataSource=:memory:;Mode=Memory;Cache=Shared;Foreign Keys=True;");
            _connection.Open();

            services.AddDbContext<AppDbContext>(options => { options.UseSqlite(_connection); });

            var sp = services.BuildServiceProvider();
            using (var scope = sp.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // RENS ALT: Dette fjerner fejl fra tidligere test-kørsler
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                SeedData(db);
            }
        });
    }

    private void SeedData(AppDbContext db)
    {
        if (db.Companies.Any())
        {
            return;
        } db.Database.EnsureCreated();
        // 1. Definer ID'er
        var myCompanyId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var otherCompanyId = Guid.Parse("99999999-9999-9999-9999-999999999999");
        var myAccountId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var otherAccountId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var forbiddenProjectId = Guid.Parse("00000000-0000-0000-0000-000000000003");
        byte[] dummyRowVersion = new byte[8] { 1, 0, 0, 0, 0, 0, 0, 0 };

        db.Database.ExecuteSqlRaw("PRAGMA foreign_keys = OFF;");

        // 2. Opret og gem firmaer
        var myCompany = new Company { Id = myCompanyId, AccountId = myAccountId, Name = "Mit Firma", CVRNumber = new CvrNumber("12345678"), Email = new EmailAddress("test@test.dk"), RowVersion = dummyRowVersion };
        var otherCompany = new Company { Id = otherCompanyId, AccountId = otherAccountId, Name = "Konkurrenten", CVRNumber = new CvrNumber("87654321"), Email = new EmailAddress("test2@test.dk"), RowVersion = dummyRowVersion };

        db.Companies.AddRange(myCompany, otherCompany);
        db.SaveChanges();

        // 3. Opret og gem permission
        db.Permissions.Add(new Permission { Id = Guid.NewGuid(), Title = "Standard Permission", RowVersion = dummyRowVersion });
        db.SaveChanges();

        // 4. Opret projekt
        db.Projects.Add(new Project
        {
            Id = forbiddenProjectId,
            Name = "Hemmeligt Projekt",
            CompanyId = otherCompanyId,
            RowVersion = dummyRowVersion,
            Address = new Address { City = "Test", Street = "Test", PostalCode = "1234", Country = "DK" }
        });
        db.SaveChanges();

        db.Database.ExecuteSqlRaw("PRAGMA foreign_keys = ON;");
    }

    protected override void Dispose(bool disposing)
    {
        _connection?.Close();
        base.Dispose(disposing);
    }
}