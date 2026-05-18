using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.External.Economic
{
    public class EconomicEmployeeResponse
    {
        public List<EconomicEmployeeDto> Items { get; set; } = new();
    }
}
