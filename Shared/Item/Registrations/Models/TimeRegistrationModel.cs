using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.Item.Registrations.Models
{
    public class TimeRegistrationModel : BaseRegistrationModel
    {
        [Required(ErrorMessage = "Starttid er påkrævet")]
        public DateTime Start { get; set; }
        [Required(ErrorMessage = "Sluttid er påkrævet")]
        public DateTime End { get; set; }
        [Required(ErrorMessage = "Tidstype er påkrævet")]
        public bool IsBreak { get; set; }
    }
}
