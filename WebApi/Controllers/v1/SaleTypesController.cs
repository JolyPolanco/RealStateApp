using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Core.Application.Dtos.SaleType;
using RealStateApp.Core.Application.Features.PropertyType.Queries.GetAllWithInclude;
using RealStateApp.Core.Application.Features.SaleType.Commands.CreateSaleType;
using RealStateApp.Core.Application.Features.SaleType.Commands.DeleteSaleType;
using RealStateApp.Core.Application.Features.SaleType.Commands.EditSaleType;
using RealStateApp.Core.Application.Features.SaleType.Queries.GetAllWithInclude;
using RealStateApp.Core.Application.Features.SaleType.Queries.GetById;
using RealStateApp.Core.Application.ViewModels.User;

namespace RealStateWebApi.Controllers.v1
{
    [Authorize(Roles = "ADMIN,DEVELOPER")]
    public class SaleTypesController : BaseApiController
    {

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Create(CreateSaleTypeCommand command)
        {

            var response = await Mediator.Send(command); 
          
          

            return Created();
        }

        [HttpPut("{Id}")]
        [Authorize(Roles = "ADMIN")]

        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult >Update(int Id, EditSaleTypeCommand command)
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

        public async Task<IActionResult >GetAllList()
        {

            var response = await Mediator.Send(new GetAllSaleTypeWithIncludeQuery());
            if (response == null || response.Count == 0)
            {
                return NoContent();
            }

            return Ok(response);
        }

        [HttpGet("{Id}")]

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<IActionResult> GetById([FromRoute] int Id)
        {
            var response = await Mediator.Send(new GetSaleTypeByIdQuery() { Id=Id});
            if (response==null) return NoContent();
            return Ok(response);
        }

        [HttpDelete("{Id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "ADMIN")]

        public async Task<IActionResult> Delete([FromRoute]int Id)
        {
             await Mediator.Send(new DeleteSaleTypeCommand() { Id = Id });
            return NoContent();
        }


    }
}
