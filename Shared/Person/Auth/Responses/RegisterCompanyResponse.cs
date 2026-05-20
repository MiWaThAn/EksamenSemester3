using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Person.Auth.Responses
{
    public record RegisterCompanyResponse : BaseResponse
    {
        public Guid Id { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
