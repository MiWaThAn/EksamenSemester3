using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Person.Auth.Responses
{
    public record RegisterEmployeeAccountResponse : BaseResponse
    {
        public Guid Id { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
