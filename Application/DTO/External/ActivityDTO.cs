using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.External
{
    public class ActivityDTO
    {
        public string Name { get; set; } = string.Empty;

        public int Number { get; set; }
        
        public int GroupNumber { get; set; }

        public bool IsBarred { get; set; }

        public bool HideInSearch { get; set; }

        public string ObjectVersion { get; set; }

    }
}
