using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface IExternalAPIService
    {




        Task <IEnumerable<CustomerDTO>> GetCustomersAsync();
        Task <IEnumerable<ProjectDTO>> GetProjectsAsync();
        Task <IEnumerable<EmployeeDTO>> GetEmployeesAsync();
    }
}
