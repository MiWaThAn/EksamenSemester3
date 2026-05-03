using Application.Interfaces;
using Application.Interfaces.Repo.Item;
using Application.Interfaces.Repo.Item.IRegistrationRepo;
using Application.Interfaces.Repo.Person;
using Domain.Interfaces.Repos;
using Infrastructure.Data;
using Infrastructure.Repositories.Item;
using Infrastructure.Repositories.Item.Registrations;
using Infrastructure.Repositories.Person;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        //Person
        public ICustomerRepository Customers { get; }
        public IEmployeeRepository Employees { get; }
        public ICompanyRepository Companies { get; }

        //Item
        public IProjectRepository Projects { get; }
        public IProjectActivityRepository ProjectActivities { get; }
        public IActivityRepository Activities { get; }
        public IAddressRepository Addresses { get; }
        public IExpenseRepository Expenses { get; }

        //Registrations
        public IHourRegistrationRepository HourRegistrations { get; }
        public IExpenseRegistrationRepository ExpenseRegistrations { get; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Customers = new CustomerRepository(_context);
            Employees = new EmployeeRepository(_context);
            Companies = new CompanyRepository(_context);

            Projects = new ProjectRepository(_context);
            ProjectActivities = new ProjectActivityRepository(_context);
            Activities = new ActivityRepository(_context);
            Addresses = new AddressRepository(_context);
            Expenses = new ExpenseRepository(_context);

            HourRegistrations = new HourRegistrationRepository(_context);
            Expenses = new ExpenseRepository(_context);
        }


        //Save changes to the database
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
