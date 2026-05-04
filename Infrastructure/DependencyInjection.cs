using Application.Interfaces;
using Domain.Interfaces.Repos;
using Infrastructure.Data;
using Infrastructure.Repositories.Person;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
        {

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();

            return services;
        }
    }
}
