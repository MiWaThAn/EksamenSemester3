using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs
{
    public class EmployeeDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string EmployeeType { get; set; }
        public bool IsAutonomous { get; set; }
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }
        public Guid? AccountId { get; set; }
        public DateTime CreatedAt { get; set; }

        public static EmployeeDTO FromEntity(Domain.Entity.Person.Employee employee)
        {
            return new EmployeeDTO
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email?.Value,
                EmployeeType = employee.EmployeeType.ToString(),
                IsAutonomous = employee.IsAutonomous,
                CompanyId = employee.CompanyId,
                CompanyName = employee.Company?.Name,
                AccountId = employee.AccountId,
                CreatedAt = employee.CreatedAt
            };
        }
    }
}
