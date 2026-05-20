using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.Person.Auth.Models.Registration
{
    public abstract class BaseRegistrationModel
    {
        [Required, EmailAddress(ErrorMessage = "Ugyldig email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Ugyldig Brugernavn")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "Brugernavn skal være mindst 8 symboler lang")]
        public string Username { get; set; }

        [Required, MinLength(12, ErrorMessage = "Password skal være mindst 12 tegn")]
        public string Password { get; set; }

        [Compare(nameof(Password), ErrorMessage = "Passwords matcher ikke")]
        public string ConfirmPassword { get; set; }
        [Required,Phone(ErrorMessage = "Telefon nummer er påkrævet")]
        public string PhoneNumber { get; set; }
    }
}
