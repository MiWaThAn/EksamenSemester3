using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.External
{
    public class CustomerDTO
    {
        public int CustomerNumber { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string PostCode { get; set; }
        public string City { get; set; }
        public string CvrNo { get; set; }
        public string ObjectVersion { get; set; } = string.Empty;
        
    }
}
