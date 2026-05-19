using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.External
{
    public class EmployeeDTO
    {
        public int Number { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; }
        public bool IsBarred { get; set; }
        public string ObjectVersion { get; set; } = string.Empty;
    }
}
