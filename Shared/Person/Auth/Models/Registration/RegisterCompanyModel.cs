using Shared.Person.Auth.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.Person.Auth.Models.Registration
{
    public class RegisterCompanyModel : BaseRegistrationModel
    {
        [Required(ErrorMessage = "Firmanavn er påkrævet")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "CVR-nummer er påkrævet")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "CVR skal være 8 tal")]
        public string CVRNumber { get; set; }

        public RegisterCompanyCommand ToCommand => new RegisterCompanyCommand(CompanyName, Password,Username,Email,PhoneNumber,CVRNumber);
    }
}
