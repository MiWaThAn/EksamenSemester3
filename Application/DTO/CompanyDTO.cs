using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs
{
    public class CompanyDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CVRNumber { get; set; }
        public Guid AccountId { get; set; }

        public List<EmployeeDTO> Employees { get; set; } = new();

        public static CompanyDTO FromEntity(Domain.Entity.Person.Company company)
        {
            return new CompanyDTO
            {
                Id = company.Id,
                Name = company.Name ?? "Unknown",
                Email = company.Email?.Value ?? "No Email",
                CVRNumber = company.CVRNumber?.Value ?? "No CVR",
                AccountId = company.AccountId,

                Employees = company.Employees?
                    .Select(e => EmployeeDTO.FromEntity(e))
                    .ToList() ?? new List<EmployeeDTO>()
            };
        }
    }
}
