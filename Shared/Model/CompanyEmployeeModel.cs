using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Model
{
    public class CompanyEmployeeModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
        public int NotificationCount { get; set; }
    }
}
