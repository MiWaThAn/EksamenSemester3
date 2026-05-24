using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Item.Registrations.Models
{
    public class ExpenseRegistrationModel : BaseRegistrationModel
    {
        public Guid ExpenseId { get; internal set; }
    }
}
