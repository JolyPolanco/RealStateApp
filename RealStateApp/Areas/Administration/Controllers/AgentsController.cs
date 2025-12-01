using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Application.ViewModels.User;
using System.Collections.Generic;

namespace RealStateApp.Areas.Administration.Controllers
{
    [Area("Administration")]
    [Authorize(Roles = "ADMIN")]
    public class AgentsController : Controller
    {
        private readonly IAdministrationService _administrationService;
        private readonly IMapper _mapper;

        public AgentsController(IAdministrationService administrationService, IMapper mapper)
        {
            _administrationService= administrationService;
            _mapper = mapper;
        }

        [HttpGet("agentes")]
        public async Task<IActionResult> Index()
        {
            var agents= await _administrationService.GetAgents();
            var vms = _mapper.Map<List<AgentViewModel>>(agents);
            return View(vms);
        }

        [HttpGet("confirmar")]
        public  IActionResult ConfirmDelete(AgentViewModel vm)
        {
        
            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult>DeleteAgent (string id)
        {
           await _administrationService.DeleteAgentAndProperties(id);

            return RedirectToAction("Index");
        }


        public IActionResult Deactivate(string id)
        {
            ViewBag.IsActivateMode = false;
            return View("ChangeState",id);
        }
        public  IActionResult Activate(string id)
        {
            ViewBag.IsActivateMode = true;

            return View("ChangeState",id);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleState(string id)
        {
            await _administrationService.ToogleState(id);

            return RedirectToAction("Index");
        }
    }
}
