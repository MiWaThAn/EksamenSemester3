using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services
{
 
        public interface IExternalAPIService
        {




            Task<IEnumerable<CustomerDTO>> GetCustomersAsync();
            Task<IEnumerable<ProjectDTO>> GetProjectsAsync();
            Task<IEnumerable<EmployeeDTO>> GetEmployeesAsync();
        }
    }

}
