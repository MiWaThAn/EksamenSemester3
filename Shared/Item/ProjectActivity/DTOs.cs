using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Item.ProjectActivity
{
    public class ProjectActivityDto
    {
        public Guid ProjectActivityId { get; set; }
        public Guid ProjectId { get; set; }
        public Guid ActivityId { get; set; }

        public string ActivityName { get; set; }
        public string ActivityDescription { get; set; }


        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid? ResponsibleEmployeeId { get; set; }
    }
    public class ProjectDto
    {
        public Guid ProjectId { get; set; }

        public string ProjectName { get; set; }
        public string Description { get; set; }


        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public Guid? ResponsibleEmployeeId { get; set; }
    }
}
