using AutoMapper;
using RealStateApp.Core.Application.Dtos.DashBoards;
using RealStateApp.Core.Application.ViewModels.Dashboards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Mappings.DtosAndViewModels
{
    public class DashboardsToViewmodelsMappingProfile: Profile
    {
        public DashboardsToViewmodelsMappingProfile()
        {
            CreateMap<AdminStatsDto, AdminDashboardViewModel>();
        }
    }
}
