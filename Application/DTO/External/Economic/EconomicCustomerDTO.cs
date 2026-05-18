using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.External.Economic
{
    public class EconomicCustomerDto
    {
        public int CustomerNumber { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Address1 { get; set; }
        public string? PostCode { get; set; }
        public string? City { get; set; }
        public string? CvrNo { get; set; }
        public string ObjectVersion { get; set; } = string.Empty;
    }
}
