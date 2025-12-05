using MediatR;
using RealStateApp.Core.Application.Dtos.User;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Domain.Common.Enums;
using RealStateApp.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.Agents.Queries.GetAllList
{
    public class GetAllAgentsListQuery : IRequest<IList<AgentDto>>
    {
    }

    public class GetAllAgentsListQueryHandler : IRequestHandler<GetAllAgentsListQuery, IList<AgentDto>>
    {
        private readonly IUserService _userService;
        private readonly IPropertyRepository _propertyRepository;

        public GetAllAgentsListQueryHandler(IUserService userService, IPropertyRepository propertyRepository)
        {
            _userService = userService;
            _propertyRepository = propertyRepository;
        }
        public async Task<IList<AgentDto>> Handle(GetAllAgentsListQuery request, CancellationToken cancellationToken)
        {
            var dictionary = await _propertyRepository.GetAgentsPropertiesCount();

            if (dictionary == null)
                throw new ApiException("Properties count data not available");

            var dtos = await _userService.GetUsersAgentOnly(dictionary);

            if (dtos == null)
                throw new ApiException("Agents data not available");

            return dtos;
        }
    }
}
