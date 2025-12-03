using RealStateApp.Core.Application.Dtos.User;

namespace RealStateApp.Core.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto> DeleteAsync(string id);
        Task<int> GetActiveAgentsCount();
        Task<int> GetActiveClientsCount();
        Task<int> GetActiveDevelopersCount();
        Task<UserDto?> GetByDni(string dni);
        Task<UserDto?> GetById(string Id);
        Task<List<UserDto>> GetClientsDevelopersOnly();
        Task<int> GetInactiveAgentsCount();
        Task<int> GetInactiveClientsCount();
        Task<int> GetInactiveDevelopersCount();
        Task<List<UserDto>> GetUsersAdminOnly();
        Task<List<AgentDto>> GetUsersAgentOnly(Dictionary<string, int> dictionary);
        Task<List<UserDto>> GetUsersByRole(string role, string displayRole);
        Task<List<UserDto>> GetUsersDevelopersOnly();
        Task<bool> SetStatus(string id, bool status);
        Task ToogleState(string id);
    }
}