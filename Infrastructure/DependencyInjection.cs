
using Application.Interfaces;
using Application.Interfaces.Adapters;
using Application.Interfaces.Data;
using Application.Interfaces.Repo.Item;
using Application.Interfaces.Repo.Item.IRegistrationRepo;
using Application.Interfaces.Repo.Mapping;
using Application.Interfaces.Repo.Person;
using Application.Interfaces.Repo.Person.Auth;
using Application.Interfaces.Services;
using Application.Interfaces.Services.Sync;
using Application.Services;
using Domain.Entity.Person;
using Domain.Interfaces;
using Domain.Interfaces.Item;
using Domain.Interfaces.Repos;
using Infrastructure.Adapters;
using Infrastructure.Adapters.Economic;
using Infrastructure.Data;
using Infrastructure.Repositories.Item;
using Infrastructure.Repositories.Item.Registrations;
using Infrastructure.Repositories.Mappings;
using Infrastructure.Repositories.Person;
using Infrastructure.Repositories.Person.Auth;
using Infrastructure.Service;
using Infrastructure.Service.Security;
using Microsoft.AspNetCore.Identity;
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

            //REPO
            //Person
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<ICompanyRepository,CompanyRepository>();
            //Auth
            services.AddScoped<IAccountRepository,AccountRepository>();
            services.AddScoped<IRoleRepository,RoleRepository>();
            services.AddScoped<IPermissionRepository,PermissionRepository>();

            //Item
            services.AddScoped<IProjectRepository,ProjectRepository>();
            services.AddScoped<IProjectActivityRepository,ProjectActivityRepository>();
            services.AddScoped<IActivityRepository,ActivityRepository>();
            services.AddScoped<IAddressRepository,AddressRepository>();
            services.AddScoped<IExpenseRepository,ExpenseRepository>();

            //Registrations
            services.AddScoped<IHourRegistrationRepository,HourRegistrationRepository>();
            services.AddScoped<IExpenseRegistrationRepository,ExpenseRegistrationRepository>();
            services.AddScoped<IWorkLogRepository,WorkLogRepository>();

            //Mappings
            services.AddScoped < IIntegrationMappingRepository,IntegrationMappingsRepository>();
            services.AddScoped<IIntegrationSettingsRepository,IntegrationSettingsRepository>();
            services.AddScoped<IProviderRepository,ProviderRepository>();


        //SERVICES
            services.AddScoped<IPasswordHasher<Account>, PasswordHasher<Account>>();
            services.AddScoped<IHashingService, HashingService>();
            services.AddScoped<IEncryptionService, EncryptionService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IExternalAPIService, ExternalAPIService>();
            services.AddScoped<IProviderAdapter, EconomicAdapter>();
            services.AddScoped<AdapterRegistry>();

            return services;
        }
    }
}
