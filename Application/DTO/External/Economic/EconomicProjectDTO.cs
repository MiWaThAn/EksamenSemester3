using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.External.Economic
{
    public class EconomicProjectDto
    {
        public int Number { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsClosed { get; set; }
        public int? CustomerNumber { get; set; }
        public int? ResponsibleEmployeeNumber { get; set; }
        public string ObjectVersion { get; set; } = string.Empty;
    }
}
