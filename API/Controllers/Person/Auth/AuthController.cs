using Application.Commands.Person;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Person.Auth.Commands;

namespace API.Controllers.Person.Auth
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(IMediator mediator) : ControllerBase
    {
        [HttpPost("register/company")]
        public async Task<IActionResult> Register([FromBody] RegisterCompanyCommand command)
        {
            var result = await mediator.Send(command);
            return result.Success ? Ok(new { id = result.Id }) : BadRequest(result.Message);
        }
        [HttpPost("register/employee")]
        public async Task<IActionResult> Register([FromBody] RegisterEmployeeAccountCommand command)
        {
            var result = await mediator.Send(command);
            return result.Success ? Ok(new { id = result.Id }) : BadRequest(result.Message);
        }
    }
}
