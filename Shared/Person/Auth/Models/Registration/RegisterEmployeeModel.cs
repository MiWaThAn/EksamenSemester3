using Shared.Person.Auth.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.Person.Auth.Models.Registration
{
    public class RegisterEmployeeModel : BaseRegistrationModel
    {
        [Required(ErrorMessage = "Medarbejder Id er påkrævet")]
        public Guid EmployeeId { get; set; }
        [Required(ErrorMessage = "Firma Id er påkrævet")]
        public Guid CompanyId { get; set; }
        public RegisterEmployeeAccountCommand ToCommand()
        {
            return new RegisterEmployeeAccountCommand(EmployeeId, Password, Username, PhoneNumber);
        }

    }
}
