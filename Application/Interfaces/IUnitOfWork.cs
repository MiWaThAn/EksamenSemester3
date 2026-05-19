using Application.Interfaces.Repo.Item;
using Application.Interfaces.Repo.Item.IRegistrationRepo;
using Application.Interfaces.Repo.Mapping;
using Application.Interfaces.Repo.Person;
using Application.Interfaces.Repo.Person.Auth;
using Domain.Interfaces.Repos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;


namespace Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {

        //Person
        ICustomerRepository Customers { get; }
        IEmployeeRepository Employees { get; }
        ICompanyRepository Companies { get; }

        //Auth
        IAccountRepository Accounts { get; }
        IRoleRepository Roles { get; }
        IPermissionRepository Permissions { get; }

        //Item
        IProjectRepository Projects { get; }
        IProjectActivityRepository ProjectActivities { get; }
        IActivityRepository Activities { get; }
        IAddressRepository Addresses { get; }
        IExpenseRepository Expenses { get; }

        //Registrations
        IHourRegistrationRepository HourRegistrations { get; }
        IExpenseRegistrationRepository ExpenseRegistrations { get; }
        IWorkLogRepository WorkLogs { get; }

        //Mappings
        IIntegrationMappingRepository Mappings { get; }
        IIntegrationSettingsRepository IntegrationSettings { get; }
        IProviderRepository Providers { get; }

        // Transaction Management
        Task BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync();
        Task<int> CompleteAsync(CancellationToken cancellationToken = default);

    }
}
