using RealStateApp.Core.Application.Dtos.User;

namespace RealStateApp.Infraestructure.Identity.Services
{
    public interface IUserService
    {
        Task<UserDto?> GetByDni(string dni);
        Task<List<UserDto>> GetClientsDevelopersOnly();
        Task<List<UserDto>> GetUsersAdminOnly();
        Task<List<UserDto>> GetUsersAgentOnly();
        Task<List<UserDto>> GetUsersAgentsOnly();
        Task<List<UserDto>> GetUsersByRole(string role, string displayRole);
        Task<List<UserDto>> GetUsersDevelopersOnly();
    }
}