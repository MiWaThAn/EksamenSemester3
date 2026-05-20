using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace Infrastructure.Data
{
    // Design-time factory used by EF tools to create AppDbContext when the application
    // host can't be built (e.g. configuration loading issues at design time).
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // Prefer an environment variable so CI/hosted environments can provide secrets.
            // Fallback to the same default used in development if the env var isn't set.
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                ?? "Server=localhost,1533;Database=EksamenDB;User Id=sa;Password=MyPass123$;TrustServerCertificate=True";

            optionsBuilder.UseSqlServer(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
