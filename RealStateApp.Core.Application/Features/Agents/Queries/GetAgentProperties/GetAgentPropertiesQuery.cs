using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RealStateApp.Core.Application.Dtos.Properties;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Domain.Interfaces;

namespace RealStateApp.Core.Application.Features.Agents.Queries.GetAgentProperties
{
    /// <summary>
    /// Query para obtener todas las propiedades de un agente específico
    /// </summary>
    public class GetAgentPropertiesQuery : IRequest<IList<PropertyApiDto>>
    {
        public required string AgentId { get; set; }
    }

    public class GetAgentPropertiesQueryHandler : IRequestHandler<GetAgentPropertiesQuery, IList<PropertyApiDto>>
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public GetAgentPropertiesQueryHandler(
            IPropertyRepository propertyRepository,
            IMapper mapper,
            IUserService userService)
        {
            _propertyRepository = propertyRepository;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<IList<PropertyApiDto>> Handle(GetAgentPropertiesQuery request, CancellationToken cancellationToken)
        {
            var properties = await _propertyRepository.GetListByAgentId(request.AgentId);
            
            var propertyDtos = new List<PropertyApiDto>();

            foreach (var property in properties)
            {
                var dto = _mapper.Map<PropertyApiDto>(property);
                
                // Obtener nombre del agente
                var agent = await _userService.GetById(property.AgentId);
                dto.AgentName = agent != null ? $"{agent.FirstName} {agent.LastName}" : "Unknown";
                
                propertyDtos.Add(dto);
            }

            return propertyDtos;
        }
    }
}
