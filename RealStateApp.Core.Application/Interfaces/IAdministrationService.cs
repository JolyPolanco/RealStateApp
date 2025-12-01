using RealStateApp.Core.Application.Dtos.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Interfaces
{
    public interface IAdministrationService
    {
        Task DeleteAgentAndProperties(string id);
        Task<List<UserDto>> GetAdministrators();
        Task<List<AgentDto>> GetAgents();
        Task<List<UserDto>> GetDevelopers();
        Task ToogleState(string id);
    }
}
