using System.Security.Claims;

namespace API.Controllers
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetAccountId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst(ClaimTypes.NameIdentifier)
                        ?? throw new UnauthorizedAccessException();

            return Guid.Parse(claim.Value);
        }
        public static Guid GetEmployeeId(this ClaimsPrincipal user)
        {
            var employeeId = Guid.Parse(user.FindFirst("employee_id")!.Value);

            return employeeId;
        }
        public static Guid GetCompanyId(this ClaimsPrincipal user)
        {
            var companyId = Guid.Parse(user.FindFirst("company_id")!.Value);

            return companyId;
        }
    }
}
