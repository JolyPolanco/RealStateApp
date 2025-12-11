using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Core.Application.Features.Improvement.Commands.CreateImprovement;
using RealStateApp.Core.Application.Features.Improvement.Commands.DeleteImprovement;
using RealStateApp.Core.Application.Features.Improvement.Commands.EditImprovement;
using RealStateApp.Core.Application.Features.Improvement.Queries.GetAllWithInclude;
using RealStateApp.Core.Application.Features.Improvement.Queries.GetById;
using RealStateApp.Core.Application.Features.Property.Commands.CreateProperty;
using RealStateApp.Core.Application.Features.PropertyType.Commands.Create;
using RealStateApp.Core.Application.Features.PropertyType.Commands.Delete;
using RealStateApp.Core.Application.Features.PropertyType.Commands.Edit;
using RealStateApp.Core.Application.Features.PropertyType.Queries.GetAllWithInclude;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;

namespace RealStateWebApi.Controllers.v1
{
    [SwaggerTag("PropertyTypes CRUD")]

    public class PropertyTypesController : BaseApiController
    {

      

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Create(CreatePropertyTypeCommand command)
        {

            var response = await Mediator.Send(command);



            return Created();
        }

        [HttpPut("{Id}")]
        [Authorize(Roles = "ADMIN")]

        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> Update(int Id, EditPropertyTypeCommand command)
        {
            if (Id != command.Id)
            {
                return BadRequest("The ID in the URL does not match the request body.");
            }
            var response = await Mediator.Send(command);



            return Ok();
        }


        [HttpGet]

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetAllList()
        {

            var response = await Mediator.Send(new GetAllPropertyTypeWithIncludeQuery());
            if (response == null || response.Count == 0)
            {
                return NoContent();
            }

            return Ok(response);
        }

        [HttpGet("{id}")]

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var response = await Mediator.Send(new GetImprovementByIdQuery() { Id = id });
            if (response == null) return NoContent();
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "ADMIN")]

        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await Mediator.Send(new DeletePropertyTypeCommand() { Id = id });
            return NoContent();
        }
    }
}
