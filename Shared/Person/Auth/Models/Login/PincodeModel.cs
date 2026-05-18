using Shared.Person.Auth.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.Person.Auth.Models.Login
{
    public class PincodeModel
    {
        [Required]
        [DisplayName("Pin")]
        [RegularExpression(@"\d{4}")]
        [StringLength(4)]
        public string Pincode { get; set; }
        public RegisterAccountPinCommand ToRegisterAccountPinCommand(string AccountId) => new RegisterAccountPinCommand(Pincode,AccountId);
        public PinLoginCommand ToPinLoginCommand(string AccountId) => new PinLoginCommand(Pincode,AccountId);
    }
}
