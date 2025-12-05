using RealStateApp.Core.Application.Dtos.Properties;

namespace RealStateApp.Core.Application.Interfaces
{
    public interface IAgentService
    {
        Task<List<AgentPropertyDto>> GetAgentProperties(string agentId);
        Task<AgentPropertyDto?> GetPropertyById(int propertyId);
        Task<SavePropertyDto> CreateProperty(SavePropertyDto dto, string agentId);
        Task UpdateProperty(SavePropertyDto dto);
        Task DeleteProperty(int propertyId);
        Task<string> GenerateUniqueCode();
    }
}
