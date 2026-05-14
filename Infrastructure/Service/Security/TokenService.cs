using Application.Interfaces.Services;
using Domain.Entity.Person;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Service.Security
{
    /// <summary>
    /// Denne service står for at lave vores jwt tokens
    /// Token pakker brugerens info ind i en pakke sammen med vores nøgle
    /// Når vi modtager den kan vi så aflæse brugerens info.
    /// Et claim er en påstand omkring hvem brugeren er feks. at brugeren har et navn, et id, osv..
    /// </summary>
    public class TokenService(IConfiguration config) : ITokenService
    {
        public string GetToken(Account account)
        {
            //først for vi vores key fra appsettings (I virkeligheden ville denne key være bedere gemt)
            //Denne key bruger vi både til at låse token of låse den op for verifecering.
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
            //Siger at den skal bruge den her krypterings algoritme til at låse vores token 
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            //Så tager vi alle permisions fra vores accounts roles sammler dem i en liste og sletter dublikater 
            var allPermissions = account.Roles.SelectMany(r => r.Permissions).Select(p => p.Title).Distinct();
            //Så laver vi vores liste af claims (info)
            var Claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, account.Id.ToString()), //<-- sub er en brugers unikke id
                new Claim(JwtRegisteredClaimNames.UniqueName, account.Username), //<-- UniqueName giver lidt sig selv
                new Claim("company_id", account.CompanyId?.ToString() ?? ""),  //<-- og så har vi id'er til info som brugeren er forbundet med
                new Claim("employee_id", account.EmployeeId?.ToString() ?? "")
            };
            //Så tilføjer vi brugerens permissions til vores claims
            foreach (var permTitle in allPermissions)
            {
                Claims.Add(new Claim("permission", permTitle));
            }
            //Tilsidst smidder vi det hele sammen og beskriver hvordan vores token skal se ud
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(Claims),
                Expires = DateTime.UtcNow.AddDays(7), //<-- hvornår den skal udløbe
                SigningCredentials = creds,
                Issuer = config["Jwt:Issuer"], //<-- Hvem der giver den (os)
                Audience = config["Jwt:Audience"] //<-- hvem den er beregnet for
            };
            //Så smider vi det ind i en handler som bygger vores token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            //og så retunere vi en crypteret token streng: eyJhbGci...
            return tokenHandler.WriteToken(token);
        }
    }
}
