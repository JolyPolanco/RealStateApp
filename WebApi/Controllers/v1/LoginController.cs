using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Core.Application.Dtos.Login;
using RealStateApp.Core.Application.Features.Login.Commands;
using RealStateApp.Core.Application.Interfaces;

namespace RealStateWebApi.Controllers.v1
{
    
        [Authorize]
        [ApiVersion("1.0")]
        public class LoginController() : BaseApiController
        {



            [AllowAnonymous]
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            [ProducesResponseType(StatusCodes.Status401Unauthorized)]

            [HttpPost("login", Name = "IniciarSesion")]
            public async Task<IActionResult> Login([FromBody] LoginCommand loginCommand)
            {
                
                

                    var result = await Mediator.Send(loginCommand);
                    if (result.HasError)
                    {
                        return Unauthorized("Usuario o contraseña incorrectos");
                    }
                    var jwt = result.AccessToken;
                    return Ok(new { jwt });

                
                

            }
        }
}
