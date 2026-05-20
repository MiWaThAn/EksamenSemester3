using Domain.Entity.Person;
using Domain.Interfaces.Repos;
using Shared.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Repo.Person
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        Task<Employee?> GetByIdWithDetailsAsync(Guid id);
        Task<List<CompanyEmployeeModel>?> GetEmployeesRelatedToProjectAsync(Guid projectId);
        Task<Employee?> GetByIdWithAccountAsync(Guid employeeId);
        Task<Employee?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default); 
    }
}
