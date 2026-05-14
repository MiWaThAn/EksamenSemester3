using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Person.Auth.Responses
{
    public record LoginResponse : BaseResponse
    {
        public string? Token { get; set; }
        public string? Username { get; set; }
    }
}
