using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.Item.Registrations.Models
{
    public class BaseRegistrationModel
    {
        [Required(ErrorMessage = "Kun en medarbejder kan lave en registrering")]
        public Guid EmployeeId { get; set; }
        [Required(ErrorMessage = "Et projekt er påkrævet")]
        public Guid ProjectId { get; set; }
        public Guid? ProjectActivityId { get; set; }
        public string Description { get; set; } = "";
    }
}
