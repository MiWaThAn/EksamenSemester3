using Domain.Entity.Item;
using Domain.Entity.Person;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface IExternalAdapter
    {


        public Task<IEnumerable<Customer>> GetCustomersAsync();
         public Task<IEnumerable<Project>> GetProjectsAsync();
         public Task<IEnumerable<Employee>> GetEmployeesAsync();


    }
}
