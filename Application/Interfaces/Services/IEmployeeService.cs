using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services
{
    public interface IEmployeeService
    {
        Task<EmployeeDTO> GetByIdAsync(Guid id);
        Task<EmployeeDTO> CreateEmployeeAsync();
        Task<IEnumerable<EmployeeDTO>> GetAllEmployeesAsync();
        Task<IEnumerable<EmployeeDTO>> GetEmployeesByCompanyIdAsync(Guid companyId);
    }
}
