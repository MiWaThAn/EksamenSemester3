using Shared.Person.Auth.Commands;
using Shared.Person.Auth.Models.Login;
using Shared.Person.Auth.Models.Registration;
using Shared.Person.Auth.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace UI.Services.Auth
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterCompany(RegisterCompanyModel model, CancellationToken ct = default);
        Task<AuthResponse> RegisterEmployee(RegisterEmployeeModel model, CancellationToken ct = default);
        Task<AuthResponse> RegisterPincode(PincodeModel pin, CancellationToken ct = default);
        Task<LoginResponse> Login(LoginModel model, CancellationToken ct = default);
        Task<LoginResponse> LoginWithPin(PincodeModel pin, CancellationToken ct = default);
        Task<LoginResponse> AutoLogin(CancellationToken ct = default);
        Task Logout();
        Task<AuthStateStatus> GetRequiredLoginState(CancellationToken ct = default);
        bool UserHasPin(string token);
        string? GetUserId(string token);
        Task<string?> GetUserIdAsync();
    }
}
