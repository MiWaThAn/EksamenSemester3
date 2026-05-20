using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Person.Auth.Responses
{
    public abstract record BaseResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
