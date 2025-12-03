using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Core.Application.Features.Agents.Commands.ChangeStatus;
using RealStateApp.Core.Application.Features.Agents.Queries.GetAllList;
using RealStateApp.Core.Application.Features.Agents.Queries.GetById;
using RealStateApp.Core.Application.Features.Improvement.Commands.CreateImprovement;
using RealStateApp.Core.Application.Features.Improvement.Commands.DeleteImprovement;
using RealStateApp.Core.Application.Features.Improvement.Commands.EditImprovement;
using RealStateApp.Core.Application.Features.Improvement.Queries.GetAllWithInclude;
using RealStateApp.Core.Application.Features.Improvement.Queries.GetById;

namespace RealStateWebApi.Controllers.v1
{
    public class AgentsController : BaseApiController
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

       

        [HttpPut("{Id}")]
        [Authorize(Roles = "ADMIN")]

        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> ChangeStatus(ChangeAgentStatusCommand command)
        {
           
            var response = await Mediator.Send(command);


            return NoContent();
        }


        [HttpGet]

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetAllList()
        {

            var response = await Mediator.Send(new GetAllAgentsListQuery());
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

        public async Task<IActionResult> GetById([FromRoute] GetAgentByIdQuery query)
        {
            var response = await Mediator.Send(query);
            if (response == null) return NoContent();
            return Ok(response);
        }


        //Falta este
        [HttpGet("AgentPropertiesDetails/{Id}")]

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<IActionResult> GetAgentProperties([FromRoute] GetAgentByIdQuery query)
        {
            var response = await Mediator.Send(query);
            if (response == null) return NoContent();
            return Ok(response);
        }
    }
}
