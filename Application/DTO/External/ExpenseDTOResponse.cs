using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.External
{
    public class ExpenseDTOResponse
    {
        public List<ExpenseDTO> Items { get; set; } = new();
    }
}
