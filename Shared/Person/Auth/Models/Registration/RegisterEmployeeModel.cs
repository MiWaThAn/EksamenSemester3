using Shared.Person.Auth.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.Person.Auth.Models.Registration
{
    public class RegisterEmployeeModel : BaseRegistrationModel
    {
        [Required(ErrorMessage = "Medarbejder er påkrævet")]
        public string EmployeeId { get; set; }
        public RegisterEmployeeAccountCommand ToCommand()=> new RegisterEmployeeAccountCommand(EmployeeId, Password, Username, PhoneNumber);

    }
}
