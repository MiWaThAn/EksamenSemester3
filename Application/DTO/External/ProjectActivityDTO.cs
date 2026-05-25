using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.External
{
    public class ProjectActivityDTO
    {
        public string? Number { get; set; }

        public string? ProjectExternalId { get; set; }

        public string? ActivityExternalId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string? ResponsibleEmployeeExternalId { get; set; }

        public bool Completed { get; set; }

        public string ObjectVersion { get; set; } = string.Empty;

        public DateTime LastUpdated { get; set; }
    }
}
