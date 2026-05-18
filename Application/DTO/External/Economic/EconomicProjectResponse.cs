using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.External.Economic
{
    public class EconomicProjectResponse
    {
        public List<EconomicProjectDto> Items { get; set; } = new();
    }
}
