using RealStateApp.Core.Application.Dtos.DashBoards;
using RealStateApp.Core.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Services
{
    public class DashboardService : IDashboardService
    {
        public readonly IUserService _userService;
        private readonly IPropertyService _propertiesService;

        public DashboardService( IUserService userService, IPropertyService propertiesService)
        {
            _userService = userService;
            _propertiesService = propertiesService;
        }


        public  async Task<AdminStatsDto >GetAdminStats()
        {
            return new AdminStatsDto
            {
                ActiveAgentsCount = await _userService.GetActiveAgentsCount(),
                InactiveAgentsCount = await _userService.GetInactiveAgentsCount(),
                ActiveDevelopersCount = await _userService.GetActiveDevelopersCount(),
                InactiveDevelopersCount = await _userService.GetInactiveAgentsCount(),
                ActiveClientsCount = await _userService.GetActiveClientsCount(),
                InactiveClientsCount = await _userService.GetInactiveClientsCount(),
                AvailablePropertiesCount = await _propertiesService.GetAvailablePropertiesCount(),
                SoldPropertiesCount = await _propertiesService.GetSoldPropertiesCount()


            };
        }

    }
}
