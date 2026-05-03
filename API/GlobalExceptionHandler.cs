using Microsoft.AspNetCore.Diagnostics;

namespace API
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            // Log fejlen her
            var response = new { Message = "Der opstod en uventet fejl." };

            // Differentier baseret på fejltype
            httpContext.Response.StatusCode = exception switch
            {
                DirectoryNotFoundException => StatusCodes.Status404NotFound,
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status500InternalServerError
            };

            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
            return true; // Fejlen er håndteret
        }
    }
}
