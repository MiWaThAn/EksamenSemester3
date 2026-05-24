using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Model
{
    public class ResetPasswordModel
    {
        public string Email { get; set; } = "";
        public string Token { get; set; } = "";
        public string NewPassword { get; set; } = "";
    }
}
