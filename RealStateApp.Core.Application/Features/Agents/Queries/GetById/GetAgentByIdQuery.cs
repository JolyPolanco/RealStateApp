using AutoMapper;
using MediatR;
using RealStateApp.Core.Application.Dtos.Improvement;
using RealStateApp.Core.Application.Dtos.User;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.Agents.Queries.GetById
{
    public class GetAgentByIdQuery : IRequest<AgentDto>
    {
        public string? Id { get; set; }

    }

    public class GetAgentByIdQueryHandler : IRequestHandler<GetAgentByIdQuery, AgentDto>
    {
        private IUserService _userService;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IMapper _mapper;

        public GetAgentByIdQueryHandler(IUserService userService, IPropertyRepository propertyRepository, IMapper mapper)
        {
            _userService = userService;
            _propertyRepository = propertyRepository;
            _mapper = mapper;
            
        }
        public async Task<AgentDto> Handle(GetAgentByIdQuery request, CancellationToken cancellationToken)
        {

            var userBase = await _userService.GetById(request.Id??"");

            var PropertiesCount = await _propertyRepository.GetAgentPropertiesCount(request.Id);

            if (userBase == null) throw new ApiException("Agent not found with Id");

            var dto = _mapper.Map<AgentDto>(userBase);
            dto.PropertiesCount = PropertiesCount;
            return dto;
        }
    }
}
