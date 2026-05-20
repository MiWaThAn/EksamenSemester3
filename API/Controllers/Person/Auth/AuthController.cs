using Application.Commands.Person;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Shared.Person.Auth.Commands;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace API.Controllers.Person.Auth
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(IMediator mediator, IConfiguration _config, ILogger<AuthController> logger) : ControllerBase
    {
        [HttpPost("register/company")]
        public async Task<IActionResult> Register([FromBody] RegisterCompanyCommand command, CancellationToken ct)
        {
            var result = await mediator.Send(command, ct);
            return result.Success ? Ok(new { id = result.Id }) : BadRequest(new ProblemDetails { Detail = result.Message });
        }
        [Authorize(Roles ="Company")]
        [HttpPost("register/employee")]
        public async Task<IActionResult> Register([FromBody] RegisterEmployeeAccountCommand command, CancellationToken ct)
        {
            var result = await mediator.Send(command, ct);
            return result.Success ? Ok(new { id = result.Id }) : BadRequest(new ProblemDetails { Detail = result.Message });
        }
        [HttpPost("register/pin")]
        public async Task<IActionResult> Register([FromBody] RegisterAccountPinCommand command, CancellationToken ct)
        {
            var result = await mediator.Send(command, ct);
            if (!result.Success)
                return BadRequest(new ProblemDetails { Detail = result.Message });
            return Ok(result);
        }
        [HttpPost("token/validate")]
        public IActionResult ValidateToken([FromBody] string token)
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest("Token is missing");

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _config["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _config["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return Ok(new { Valid = true });
            }
            catch (SecurityTokenExpiredException ex)
            {
                //hvis token er udløbet (normalt)
                logger.LogInformation("Token validation failed: Token has expired.");
                return Unauthorized(new ProblemDetails { Detail = "Token has expired" });
            }
            catch (Exception ex)
            {
                //andre kritiske fejl (feks. hvis nogen prøver at hacke signaturen)
                logger.LogWarning(ex, "Token validation failed due to an anomaly.");
                return Unauthorized(new ProblemDetails { Detail = "Invalid token structure or signature" });
            }
        }
        [HttpPost("login-pin")]
        public async Task<IActionResult> LoginWithPin([FromBody] PinLoginCommand command, CancellationToken ct)
        {
            var result = await mediator.Send(command, ct);

            if (!result.Success)
                return Unauthorized(new ProblemDetails { Detail = result.Message });

            return Ok(result);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken ct)
        {
            var result = await mediator.Send(command, ct);

            if (!result.Success)
            {
                return Unauthorized(new ProblemDetails { Detail = result.Message });
            }
            return Ok(result);
        }
    }
}
