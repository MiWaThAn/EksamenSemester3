using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Model
{
    public class DetailedEmployeeModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsLocal { get; set; }
        public List<CompanyProjectModel> Projects { get; set; } = new List<CompanyProjectModel>();
    }
}
