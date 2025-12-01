using RealStateApp.Core.Application.Dtos.User;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Services
{
    public class AdministrationService:IAdministrationService
    {
        private IUserService _userService;
        private IPropertyRepository _propertyRepository;

        public AdministrationService(IUserService userService, IPropertyRepository propertyRepository)
        {
            _userService = userService;
            _propertyRepository=propertyRepository;
        }



        public async Task DeleteAgentAndProperties(string id)
        {
           await _userService.DeleteAsync(id);
            await _propertyRepository.DeleteAgentProperties(id);

        }
        public async Task ToogleState(string id)
        {
            await _userService.ToogleState(id);

        }

        
        public async  Task<List<AgentDto>>GetAgents()
        {
            var dictionary = await _propertyRepository.GetAgentsPropertiesCount();
            return await _userService.GetUsersAgentOnly(dictionary);
        }

        public async Task<List<UserDto>> GetDevelopers()
        {
            return await _userService.GetUsersDevelopersOnly();
        }

        public async Task<List<UserDto>> GetAdministrators()
        {
            return await _userService.GetUsersAdminOnly();
        }
    }
}
