using Domain.Entity.Person;
using Domain.Interfaces.Repos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Repo.Person
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        Task<Employee?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default); 
    }
}
