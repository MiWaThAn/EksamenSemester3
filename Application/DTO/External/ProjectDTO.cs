using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.External
{
    public class ProjectDTO
    {
        public int Number { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsClosed { get; set; }
        public int CustomerNumber { get; set; }
        public int ResponsibleEmployeeNumber { get; set; }
        public string ObjectVersion { get; set; } = string.Empty;
        
    }
}
