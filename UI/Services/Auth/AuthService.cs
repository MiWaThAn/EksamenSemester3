using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Shared.Person.Auth.Commands;
using Shared.Person.Auth.Models.Login;
using Shared.Person.Auth.Models.Registration;
using Shared.Person.Auth.Responses;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using UI.Services.Auth.Registration;

namespace UI.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _http;
        private readonly JwtAuthStateProvider _authStateProvider;
        private readonly ISecureStorage _secureStorage;
        private readonly PushRegistrationService _pushRegistrationService;
        public AuthService(HttpClient httpClient, JwtAuthStateProvider authStateProvider,ISecureStorage secureStorage, PushRegistrationService pushRegistrationService)
        {
            _http = httpClient;
            _authStateProvider = authStateProvider;
            _secureStorage = secureStorage;
            _pushRegistrationService = pushRegistrationService;
        }

        /// <summary>
        /// metode til registrering af firmaer
        /// Den modtager registrerings modellen og transformerer den til en commando
        /// Den commando sendes til vores api lag som sender et response
        /// hvis der ikke er forbindelse til serveren kaster den en error
        /// </summary>
        public async Task<AuthResponse> RegisterCompany(RegisterCompanyModel model, CancellationToken ct = default)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/auth/register/company", model.ToCommand, ct);
                if (response.IsSuccessStatusCode)
                    return new AuthResponse { Success = true };

                var error = await response.Content.ReadFromJsonAsync<ProblemDetails>(ct);
                return new AuthResponse { Success = false, Message = error?.Detail ?? "Fejl ved registrering" };
            }
            catch (Exception)
            {
                return new AuthResponse { Success = false, Message = "Kunne ikke oprette forbindelse til serveren." };
            }
        }
        public async Task<AuthResponse> RegisterEmployee(RegisterEmployeeModel model, CancellationToken ct = default)
        {
            await EnsureAuthorizationHeaderAsync();
            try
            {
                var response = await _http.PostAsJsonAsync("api/auth/register/employee", model.ToCommand, ct);
                if (response.IsSuccessStatusCode)
                    return new AuthResponse { Success = true };

                var error = await response.Content.ReadFromJsonAsync<ProblemDetails>(ct);
                return new AuthResponse { Success = false, Message = error?.Detail ?? "Fejl ved registrering" };
            }
            catch (Exception)
            {
                return new AuthResponse { Success = false, Message = "Kunne ikke oprette forbindelse til serveren." };
            }
        }
        public async Task<AuthResponse> RegisterPincode(PincodeModel pin, CancellationToken ct = default)
        {
            await EnsureAuthorizationHeaderAsync();
            var token = await _secureStorage.GetAsync("auth_token");
            if (string.IsNullOrWhiteSpace(token))
                return new AuthResponse { Success = false, Message = "Ingen aktiv session fundet." };

            var claims = _authStateProvider.ParseClaimsFromJwt(token);
            var accountId = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub || c.Type == "sub")?.Value;

            if (string.IsNullOrWhiteSpace(accountId))
                return new AuthResponse { Success = false, Message = "Konto id var ikke tilknyttet" };

            try
            {
                var response = await _http.PostAsJsonAsync("api/auth/register/pin", pin.ToRegisterAccountPinCommand(accountId), ct);
                var result = await response.Content.ReadFromJsonAsync<RegisterAccountPinResponse>(ct);

                if (response.IsSuccessStatusCode && result?.Token != null)
                {
                    await _secureStorage.SetAsync("auth_token", result.Token);
                    _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.Token);
                    _authStateProvider.NotifyLogin(result.Token);

                    return new AuthResponse { Success = true };
                }

                var error = await response.Content.ReadFromJsonAsync<ProblemDetails>(ct);
                return new AuthResponse { Success = false, Message = error?.Detail ?? "Fejl" };
            }
            catch (Exception)
            {
                return new AuthResponse { Success = false, Message = "Netværksfejl under oprettelse af PIN." };
            }
        }
        public async Task<LoginResponse> Login(LoginModel model, CancellationToken ct = default)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/auth/login", model.ToCommand, ct);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>(ct);
                    if (result?.Token != null)
                    {
                        await _secureStorage.SetAsync("auth_token", result.Token);
                        await _secureStorage.SetAsync("last_full_login", DateTime.UtcNow.ToString("O"));
                        await _secureStorage.SetAsync("last_pin_login", DateTime.UtcNow.ToString("O"));

                        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.Token);
                        _authStateProvider.NotifyLogin(result.Token);
                        try
                        {
                            if(result.AccountId != Guid.Empty)
                                await _pushRegistrationService.RegisterDeviceAsync(result.AccountId);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Push-registrering fejlede i baggrunden: {ex.Message}");
                        }
                        return result;
                    }
                }
                try
                {
                    var errorDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>(ct);
                    return new LoginResponse { Message = errorDetails?.Detail ?? "Forkert brugernavn eller adgangskode.", Success = false };
                }
                catch
                {
                    return new LoginResponse { Message = "Fejl ved login (Serveren svarede med fejlkode: " + response.StatusCode + ")", Success = false };
                }
            }
            catch (HttpRequestException)
            {
                return new LoginResponse { Message = "Ingen netværksforbindelse til serveren.", Success = false };
            }
            catch (Exception ex)
            {
                //til fejlfinding under eksamensprojektet
                return new LoginResponse { Message = $"Uventet fejl: {ex.Message}", Success = false };
            }
        }
        public async Task Logout()
        {
            _secureStorage.Remove("auth_token");
            _secureStorage.Remove("last_full_login");
            _secureStorage.Remove("last_pin_login");

            _http.DefaultRequestHeaders.Authorization = null;
            _authStateProvider.NotifyLogout();
        }

        /// <summary>
        /// For auto login skal vi finde vores token, smidde den på vores clients header
        /// og opdatere vores ui igennem vores jwt provider.
        /// </summary>
        public async Task<LoginResponse> AutoLogin(CancellationToken ct = default)
        {
            var token = await _secureStorage.GetAsync("auth_token");
            if (string.IsNullOrWhiteSpace(token))
            {
                return new LoginResponse { Success = false, Message = "Ingen gemt session fundet." };
            }

            try
            {
                var response = await _http.PostAsJsonAsync("api/auth/token/validate", token, ct);
                if (!response.IsSuccessStatusCode)
                {
                    return new LoginResponse { Success = false, Message = "Token er udløbet." };
                }

                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                _authStateProvider.NotifyLogin(token);
                try
                {
                    var accountId = GetUserId(token);
                    if (Guid.TryParse(accountId, out Guid userGuid) && userGuid != Guid.Empty)
                    {
                        if (userGuid != Guid.Empty)
                            await _pushRegistrationService.RegisterDeviceAsync(userGuid,ct);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Push-registrering fejlede i baggrunden: {ex.Message}");
                }
                return new LoginResponse { Success = true, Token = token };
            }
            catch (Exception)
            {
                return new LoginResponse { Success = false, Message = "Kunne ikke validere offline." };
            }
        }




        public async Task<LoginResponse> LoginWithPin(PincodeModel pin, CancellationToken ct = default)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/auth/login-pin", pin.ToPinLoginCommand, ct);
                if (response.IsSuccessStatusCode)
                {
                    await _secureStorage.SetAsync("last_pin_login", DateTime.UtcNow.ToString("O"));
                    var token = await _secureStorage.GetAsync("auth_token");

                    _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    _authStateProvider.NotifyLogin(token);

                    return new LoginResponse { Success = true };
                }
                return new LoginResponse { Success = false, Message = "Forkert PIN-kode." };
            }
            catch (Exception)
            {
                return new LoginResponse { Success = false, Message = "Netværksfejl." };
            }
        }





        public async Task<AuthStateStatus> GetRequiredLoginState(CancellationToken ct = default)
        {
            //hvis der ikke er en token betyder det at ingen session er gemt og at de skal logge ind.
            var token = await _secureStorage.GetAsync("auth_token");
            if (string.IsNullOrEmpty(token)) return AuthStateStatus.NeedsFullLogin;
            
            //hvis det er 50 dage siden de sidst logget ind så skal de logge ind
            var lastFullStr = await _secureStorage.GetAsync("last_full_login");
            if (DateTime.TryParse(lastFullStr, out var lastFull))
            {
                if ((DateTime.UtcNow - lastFull).TotalDays >= 50)
                    return AuthStateStatus.NeedsFullLogin;
            }
            else
            {
                return AuthStateStatus.NeedsFullLogin;
            }

            try
            {
                //hvis token er er valid skal de logge ind igen da den enten er udløbet eller er en forfalskning
                var response = await _http.PostAsJsonAsync("api/auth/token/validate", token, ct);
                if (!response.IsSuccessStatusCode)
                {
                    return AuthStateStatus.NeedsFullLogin;
                }
            }
            catch (Exception)
            {
                //hvis serveren er nede sender vi dem til fuldt login som sikkerhedsforanstaltning
                return AuthStateStatus.NeedsFullLogin;
            }
            //hvis de har en pin
            var claims = _authStateProvider.ParseClaimsFromJwt(token);
            var hasPinClaim = claims.FirstOrDefault(c => c.Type == "has_pin")?.Value;
            bool userHasPin = hasPinClaim == "true";
            //og hvis deres sidste login med pin er for 7 dage siden, så skal de logge ind med pin igen
            var lastPinStr = await _secureStorage.GetAsync("last_pin_login");
            DateTime.TryParse(lastPinStr, out var lastPin);

            var daysSincePin = (DateTime.UtcNow - lastPin).TotalDays;
            if (daysSincePin >= 7)
            {
                return userHasPin ? AuthStateStatus.NeedsPinLogin : AuthStateStatus.NeedsFullLogin;
            }
            return AuthStateStatus.Authorized;
        }
        //hjælpemetode til at sikre at Httpclient har sit token på sig inden kald.
        private async Task EnsureAuthorizationHeaderAsync()
        {
            if (_http.DefaultRequestHeaders.Authorization == null)
            {
                var token = await _secureStorage.GetAsync("auth_token");
                if (!string.IsNullOrWhiteSpace(token))
                {
                    _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }
        }
        public bool UserHasPin(string token)
        {
            var claims = _authStateProvider.ParseClaimsFromJwt(token);
            var hasPinClaim = claims.FirstOrDefault(c => c.Type == "has_pin")?.Value;
            return hasPinClaim == "true";
        }
        public string? GetUserId(string token)
        {
            var claims = _authStateProvider.ParseClaimsFromJwt(token);
            var accountId = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub || c.Type == "sub")?.Value;
            return accountId;
        }
    }
}
