using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using RealStateApp.Core.Application.Features.Property.Queries.GetAll;
using RealStateApp.Core.Application.Features.Property.Queries.GetAllWithInclude;
using RealStateApp.Core.Application.Features.Property.Queries.GetByCode;
using RealStateApp.Core.Application.Features.Property.Queries.GetById;

namespace RealStateWebApi.Controllers.v1
{
    /// <summary>
    /// Controlador de Propiedades para la API
    /// Versión: 1.0
    /// </summary>
    public class PropertiesController : BaseApiController
    {
        /// <summary>
        /// Obtiene todas las propiedades del sistema
        /// </summary>
        /// <returns>Lista de todas las propiedades</returns>
        [HttpGet]
        [Authorize(Roles = "ADMIN,DEVELOPER")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            var response = await Mediator.Send(new GetAllPropertiesQuery());

            if (response == null || response.Count == 0)
            {
                return NoContent();
            }

            return Ok(response);
        }

        /// <summary>
        /// Obtiene todas las propiedades con sus relaciones incluidas
        /// </summary>
        /// <returns>Lista de propiedades con PropertyType, SaleType e Improvements</returns>
        [HttpGet("withinclude")]
        [Authorize(Roles = "ADMIN,DEVELOPER")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllWithInclude()
        {
            var response = await Mediator.Send(new GetAllPropertiesWithIncludeQuery());

            if (response == null || response.Count == 0)
            {
                return NoContent();
            }

            return Ok(response);
        }

        /// <summary>
        /// Obtiene una propiedad por su Id
        /// </summary>
        /// <param name="id">Id de la propiedad</param>
        /// <returns>Datos de la propiedad</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "ADMIN,DEVELOPER")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var response = await Mediator.Send(new GetPropertyByIdQuery { Id = id });

            if (response == null)
            {
                return NoContent();
            }

            return Ok(response);
        }

        /// <summary>
        /// Obtiene una propiedad por su Código
        /// </summary>
        /// <param name="code">Código de la propiedad (6 caracteres)</param>
        /// <returns>Datos de la propiedad</returns>
        [HttpGet("code/{code}")]
        [Authorize(Roles = "ADMIN,DEVELOPER")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByCode([FromRoute] string code)
        {
            var response = await Mediator.Send(new GetPropertyByCodeQuery { Code = code });

            if (response == null)
            {
                return NoContent();
            }

            return Ok(response);
        }

    }
}
