using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Model
{
    public class DetailedProjectModel
    {
        public Guid Id { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
        public int NotificationCount { get; set; }

        // Listen over medarbejdere tilkoblet projektet. Vi genbruger din eksisterende model!
        public List<CompanyEmployeeModel> Employees { get; set; } = new List<CompanyEmployeeModel>();
    }
}
