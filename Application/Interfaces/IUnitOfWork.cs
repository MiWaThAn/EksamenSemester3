using Application.Interfaces.Repo.Item;
using Application.Interfaces.Repo.Item.IRegistrationRepo;
using Application.Interfaces.Repo.Mapping;
using Application.Interfaces.Repo.Person;
using Domain.Interfaces.Repos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {

        //Person
        ICustomerRepository Customers { get; }
        IEmployeeRepository Employees { get; }
        ICompanyRepository Companies { get; }
        IAccountRepository Accounts { get; }

        //Item
        IProjectRepository Projects { get; }
        IProjectActivityRepository ProjectActivities { get; }
        IActivityRepository Activities { get; }
        IAddressRepository Addresses { get; }
        IExpenseRepository Expenses { get; }

        //Registrations
        IHourRegistrationRepository HourRegistrations { get; }
        IExpenseRegistrationRepository ExpenseRegistrations { get; }


        //Mappings
        IIntegrationMappingRepository Mappings { get; }
        IIntegrationSettingsRepository IntegrationSettings { get; }


        // Transaction Management
        Task BeginTransactionAsync(IsolationLevel isolationLevel);
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();

    }
}
