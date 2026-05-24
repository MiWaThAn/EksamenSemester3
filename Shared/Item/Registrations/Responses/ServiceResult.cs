using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Item.Registrations.Responses
{
    public class ServiceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Guid? Id { get; set; }

        public static ServiceResult Ok(Guid? id = null) => new ServiceResult { Success = true, Id = id };
        public static ServiceResult Failure(string message) => new ServiceResult { Success = false, Message = message };
    }

    public class ApiResponseData
    {
        public Guid? Id { get; set; }
    }

    public class ProblemDetailsDto
    {
        public string Detail { get; set; }
    }
}
