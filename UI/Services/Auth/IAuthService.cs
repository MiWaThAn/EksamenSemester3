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
        Task<AuthResponse> RegisterCompany(RegisterCompanyModel model);
        Task<AuthResponse> RegisterEmployee(RegisterEmployeeModel model);
        Task<LoginResponse> Login(LoginModel model);
        Task<LoginResponse> AutoLogin();
        Task Logout();
    }
}
