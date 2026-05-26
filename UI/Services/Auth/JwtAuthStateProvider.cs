using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace UI.Services.Auth
{
    /// En lille koge allegori for beder forståelse:
    /// Jwt indeholder brugerens info og en signatur som er lidt ligesom en smoothy.
    /// Denne smoothy blev lavt ved at vi tog alle vores ingredienser (brugerens info)
    /// og en hemmelig ingrediens (vores key) og smed dem allesammen ind i en blender (algoritmen) 
    /// når api laget sender denne token er det ligesom at sende ingridientserne (brugerens info) 
    /// og en lille smagsprøve på vores smoothy (signaturen) men uden den hemmelig ingrediens (vores key). 
    /// hvis en bruger formår at ændre en af ingridenserne i deres client (feks. at give dem selv admin rolle)
    /// så når den token modtages af api laget, blender den alle ingridenterne sammen igen og gør en lille smagstest
    /// Hvis den nye smag (signatur) ikke matcher den gamle (signaturen der kom med deres token) 
    /// ved den så at der er fusk på spil.
    /// Alle kan altså se brugernes info men kun serveren kan ændre og validere dem. 
    public class JwtAuthStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());
        public JwtAuthStateProvider(HttpClient httpClient)
        {
           _httpClient = httpClient;
        }
        //denne metode kaldes automatisk af appen for at finde ud af hvem brugeren er 
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await SecureStorage.Default.GetAsync("auth_token");
                //hvis der ikke er en token så er de ikke registrert
                if (string.IsNullOrWhiteSpace(token)) 
                    return new AuthenticationState(_anonymous);
                //hvis de har en token sætter vi den på deres client så api kald altid har dem med.
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var claims = ParseClaimsFromJwt(token);
                // Notice the two extra parameters at the end!
                var identity = new ClaimsIdentity(claims, "jwt", "name", ClaimTypes.Role);
                return new AuthenticationState(new ClaimsPrincipal(identity));
            }
            catch
            {
                return new AuthenticationState(_anonymous);
            }
        }
        //kaldes når en bruger logger ind
        public void NotifyLogin(string token)
        {
            var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt", "name", ClaimTypes.Role);
            var user = new ClaimsPrincipal(identity);
            var state = Task.FromResult(new AuthenticationState(user));

            NotifyAuthenticationStateChanged(state);
        }
        // Kaldes ved logud
        public void NotifyLogout()
        {
            var state = Task.FromResult(new AuthenticationState(_anonymous));
            NotifyAuthenticationStateChanged(state);
        }
        /// <summary>
        /// Hjælpemetode til at læse claims uden at validere signaturen (det gør API'et)
        /// Jwt er skrevet i base64 så vi oversætter vores token for at få brugerens info
        /// </summary>
        public IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            if (keyValuePairs == null) return Enumerable.Empty<Claim>();

            var claims = new List<Claim>();

            // 1. UNIFY ROLE PARSING (Check both "role" and the long XML URI string)
            string roleKey = keyValuePairs.ContainsKey("role") ? "role" : ClaimTypes.Role;

            if (keyValuePairs.TryGetValue(roleKey, out var roles))
            {
                var rolesStr = roles.ToString()!.Trim();
                if (rolesStr.StartsWith("["))
                {
                    var parsedRoles = JsonSerializer.Deserialize<string[]>(rolesStr);
                    foreach (var role in parsedRoles!)
                        claims.Add(new Claim(ClaimTypes.Role, role)); // Always map to standard ClaimTypes.Role
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, rolesStr));
                }

                keyValuePairs.Remove(roleKey);
                if (keyValuePairs.ContainsKey(ClaimTypes.Role)) keyValuePairs.Remove(ClaimTypes.Role);
            }

            // 2. PARSE PERMISSIONS
            if (keyValuePairs.TryGetValue("permission", out var perms))
            {
                if (perms.ToString()!.Trim().StartsWith("["))
                {
                    var parsedPerms = JsonSerializer.Deserialize<string[]>(perms.ToString()!);
                    foreach (var p in parsedPerms!) claims.Add(new Claim("permission", p));
                }
                else claims.Add(new Claim("permission", perms.ToString()!));

                keyValuePairs.Remove("permission");
            }

            // Add remaining fields (sub, exp, etc.)
            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!)));

            return claims;
        }
        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}
