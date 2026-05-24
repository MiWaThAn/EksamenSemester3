using Shared.Person.Auth.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Item.Registrations.Responses
{
    public class BaseRegistrationResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public Guid? Id { get; set; }

        public Dictionary<string, string[]>? Errors { get; set; }

        public static BaseRegistrationResponse Fail(string message) => new() { Success = false, Message = message };
        public static BaseRegistrationResponse Ok(Guid id) => new() { Success = true, Id = id };
    }
}
