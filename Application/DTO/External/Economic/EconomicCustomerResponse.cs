using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.External.Economic
{
    public class EconomicCustomerResponse
    {
        public List<EconomicCustomerDto> Items { get; set; } = new();
    }
}
