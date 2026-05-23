using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Application.DTO.External
{
    public class ExpenseDTO
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public bool IsBarred { get; set; }
        public string ObjectVersion { get; set; }

    }
}
