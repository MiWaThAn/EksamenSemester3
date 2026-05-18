using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Person.Auth.Responses
{
    public class AuthResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public Guid? Id { get; set; }

        public Dictionary<string, string[]>? Errors { get; set; }

        public static AuthResponse Fail(string message) => new() { Success = false, Message = message };
        public static AuthResponse Ok(Guid id) => new() { Success = true, Id = id };
    }
}
