using Application.Interfaces.Repo.Item;
using Application.Interfaces.Repo.Item.IRegistrationRepo;
using Application.Interfaces.Repo.Person;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {

        //Person
        ICustomerRepository Customers { get; }
        IEmployeeRepository Employees { get; }
        ICompanyRepository Companies { get; }

        //Item
        IProjectRepository Projects { get; }
        IProjectActivityRepository ProjectActivities { get; }
        IActivityRepository Activities { get; }
        IAddressRepository Addresses { get; }
        IExpenseRepository Expenses { get; }

        //Registrations
        IHourRegistrationRepository HourRegistrations { get; }
        IExpenseRegistrationRepository ExpenseRegistrations { get; }



        //Save changes to the database
        Task<int> CompleteAsync();

        void Dispose();
    }
}
