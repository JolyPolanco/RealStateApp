using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Application.ViewModels.Dashboards;
using System.Threading.Tasks;

namespace RealStateApp.Areas.Administration.Controllers
{

    [Area("Administration")]
    [Authorize(Roles = "ADMIN")]

    public class HomeController : Controller
    {
        private IDashboardService _dashboardService;
        private readonly IMapper _mapper;

        public HomeController(IDashboardService dashboardService, IMapper mapper)
        {
            _dashboardService=dashboardService;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
          var statsDto= await  _dashboardService.GetAdminStats();
            return View(_mapper.Map<AdminDashboardViewModel>(statsDto));
        }
    }
}
