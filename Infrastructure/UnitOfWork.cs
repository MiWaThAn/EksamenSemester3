
using Application.Interfaces;
using Application.Interfaces.Repo.Item;
using Application.Interfaces.Repo.Item.IRegistrationRepo;
using Application.Interfaces.Repo.Mapping;
using Application.Interfaces.Repo.Person;
using Domain.Entity.Mapping;
using Domain.Interfaces.Repos;
using Infrastructure.Data;
using Infrastructure.Repositories.Item;
using Infrastructure.Repositories.Item.Registrations;
using Infrastructure.Repositories.Mappings;
using Infrastructure.Repositories.Person;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Application.Interfaces.Repo.Person.Auth;
using Infrastructure.Repositories.Person.Auth;

namespace Infrastructure
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction _currentTransaction;

        //Person
        public ICustomerRepository Customers { get; }
        public IEmployeeRepository Employees { get; }
        public ICompanyRepository Companies { get; }
        //Auth
        public IAccountRepository Accounts { get; }
        public IRoleRepository Roles { get; }
        public IPermissionRepository Permissions { get; }

        //Item
        public IProjectRepository Projects { get; }
        public IProjectActivityRepository ProjectActivities { get; }
        public IActivityRepository Activities { get; }
        public IAddressRepository Addresses { get; }
        public IExpenseRepository Expenses { get; }

        //Registrations
        public IHourRegistrationRepository HourRegistrations { get; }
        public IExpenseRegistrationRepository ExpenseRegistrations { get; }

        //Mappings
        public IIntegrationMappingRepository Mappings { get; }
        public IIntegrationSettingsRepository IntegrationSettings { get; }
        public IProviderRepository Providers { get; }
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Customers = new CustomerRepository(_context);
            Employees = new EmployeeRepository(_context);
            Companies = new CompanyRepository(_context);
            Accounts = new AccountRepository(_context);
            Roles = new RoleRepository(_context);
            Permissions = new PermissionRepository(_context);

            Projects = new ProjectRepository(_context);
            ProjectActivities = new ProjectActivityRepository(_context);
            Activities = new ActivityRepository(_context);
            Addresses = new AddressRepository(_context);
            Expenses = new ExpenseRepository(_context);

            HourRegistrations = new HourRegistrationRepository(_context);
            ExpenseRegistrations = new ExpenseRegistrationRepository(_context);

            Mappings = new IntegrationMappingsRepository(_context);
            IntegrationSettings = new IntegrationSettingsRepository(_context);
            Providers = new ProviderRepository(_context);
        }

        public async Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null) return;
            _currentTransaction = await _context.Database.BeginTransactionAsync(isolationLevel, cancellationToken);
        }
        //Save changes to the database
        public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.CommitAsync(cancellationToken);
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
        public async Task RollbackTransactionAsync()
        {
            try
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.RollbackAsync();
                }
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }
            }
        }
        
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
