using System;
using System.Collections.Generic;

namespace Shared.Model
{
    public class DetailedProjectModel
    {
        public Guid Id { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "Åben";
        public string CustomerName { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
        public int NotificationCount { get; set; }

        public List<CompanyEmployeeModel> Employees { get; set; } = new List<CompanyEmployeeModel>();
        public List<ProjectActivityModel> Activities { get; set; } = new List<ProjectActivityModel>();
    }

    public class ProjectActivityModel
    {
        public Guid Id { get; set; }
        public string ActivityName { get; set; } = string.Empty;
        public string ActivityNumber { get; set; } = string.Empty;
        public string Status { get; set; } = "Åben"; // F.eks. "Åben", "Lukket", "Godkendes"
        public string StatusText { get; set; } = "Åben";
        public double TimeEstimate { get; set; }
    }
}