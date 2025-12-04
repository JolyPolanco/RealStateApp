using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Application.ViewModels.Properties;
using RealStateApp.Infraestructure.Identity.Entities;

namespace RealStateApp.Areas.Agents.Controllers
{
    [Area("Agents")]
    [Authorize(Roles = "AGENT")]
    public class HomeController : Controller
    {
        private readonly IAgentService _agentService;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public HomeController(IAgentService agentService, IMapper mapper, UserManager<AppUser> userManager)
        {
            _agentService = agentService;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Login", new { area = "" });
            }

            var properties = await _agentService.GetAgentProperties(user.Id);
            var viewModels = _mapper.Map<List<AgentPropertyViewModel>>(properties);

            return View(viewModels);
        }
    }
}
