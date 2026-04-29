using Domain.Entity.Person;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Repo.Person
{
    public interface IEmployeeRepository : IAccountRepository<Employee>
    {
        Task<Employee?> GetByEmployeeNumberAsync(string employeeNumber);
        Task<Employee?> GetByIdWithDetailsAsync(Guid id); 
    }
}
