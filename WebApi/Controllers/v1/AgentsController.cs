using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Core.Application.Features.Agents.Commands.ChangeStatus;
using RealStateApp.Core.Application.Features.Agents.Commands.DeleteAgent;
using RealStateApp.Core.Application.Features.Agents.Queries.GetAgentProperties;
using RealStateApp.Core.Application.Features.Agents.Queries.GetAllList;
using RealStateApp.Core.Application.Features.Agents.Queries.GetById;

namespace RealStateWebApi.Controllers.v1
{

    public class AgentsController : BaseApiController
    {
        [HttpGet]
        [Authorize(Roles = "ADMIN,DEVELOPER")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> List()
        {
            var response = await Mediator.Send(new GetAllAgentsListQuery());
            
            if (response == null || response.Count == 0)
            {
                return NoContent();
            }

            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "ADMIN,DEVELOPER")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById([FromRoute] string id)
        {
            var response = await Mediator.Send(new GetAgentByIdQuery { Id = id });
            
            if (response == null)
            {
                return NoContent();
            }

            return Ok(response);
        }

        [HttpGet("{id}/properties")]
        [Authorize(Roles = "ADMIN,DEVELOPER")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAgentProperty([FromRoute] string id)
        {
            var response = await Mediator.Send(new GetAgentPropertiesQuery { AgentId = id });
            
            if (response == null || response.Count == 0)
            {
                return NoContent();
            }

            return Ok(response);
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangeStatus([FromRoute] string id, [FromBody] ChangeAgentStatusCommand command)
        {
            command.Id = id;
            await Mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            await Mediator.Send(new DeleteAgentCommand { Id = id });
            return NoContent();
        }
    }
}
