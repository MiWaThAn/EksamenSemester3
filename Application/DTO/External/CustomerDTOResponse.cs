using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.External
{
    public class CustomerDTOResponse
    {
        public List<CustomerDTO> Items { get; set; } = new();
    }
}
