using Domain.Entity.Person;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity.Item
{
    public class ProjectAssignment : Base
    {
        public Guid ProjectId { get; private set; }
        public Guid EmployeeId { get; private set; }
        public DateTime AssignedAt { get; private set; }

        protected ProjectAssignment() { }

        public ProjectAssignment(Project project, Employee employee)
        {
            ProjectId = project.Id;
            EmployeeId = employee.Id;
            AssignedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
