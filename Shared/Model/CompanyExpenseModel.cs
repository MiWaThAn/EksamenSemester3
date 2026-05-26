using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Model
{
    public class CompanyExpenseModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
        public int NotificationCount { get; set; }
    }
}
