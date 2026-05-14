using Microsoft.AspNetCore.Mvc;
using Shared.Person.Auth.Models.Login;
using Shared.Person.Auth.Models.Registration;
using Shared.Person.Auth.Responses;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace UI.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _http;
        public AuthService(HttpClient httpClient)
        {
            _http = httpClient;
        }
        public async Task<AuthResponse> RegisterCompany(RegisterCompanyModel model)
        {
            var response = await _http.PostAsJsonAsync("api/auth/register/company", model);
            if (response.IsSuccessStatusCode)
                return new AuthResponse { Success = true };


            var error = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            return new AuthResponse { Success = false, Message = error?.Detail ?? "Fejl" };
        }
        public async Task<AuthResponse> RegisterEmployee(RegisterEmployeeModel model)
        {
            var response = await _http.PostAsJsonAsync("api/auth/register/employee", model);
            if (response.IsSuccessStatusCode)
                return new AuthResponse { Success = true };


            var error = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            return new AuthResponse { Success = false, Message = error?.Detail ?? "Fejl" };
        }
        public async Task<LoginResponse> Login(LoginModel model)
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", model);
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (response.IsSuccessStatusCode && result?.Token != null)
            {
                await SecureStorage.Default.SetAsync("auth_token", result.Token);
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.Token);
                return result;
            }
            return result ?? new LoginResponse {Message = "Fejl ved login",Success=false};
        }
        public async Task Logout()
        {
            SecureStorage.Default.Remove("auth_token");
            _http.DefaultRequestHeaders.Authorization = null;
            if (_authStateProvider is JwtAuthStateProvider jwtProvider)
            {
                jwtProvider.NotifyUserLogout();
            }
        }
    }
}
