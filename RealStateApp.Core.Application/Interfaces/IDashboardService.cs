using RealStateApp.Core.Application.Dtos.DashBoards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<AdminStatsDto> GetAdminStats();
    }
}
