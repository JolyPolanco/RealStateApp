using AutoMapper;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Application.ViewModels.User;
using System.Collections.Generic;

namespace RealStateApp.Areas.Administration.Controllers
{
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


        public async Task<IActionResult> Deactivate(string id)
        {
            await _administrationService.ToogleState(id);
            ViewBag.IsActivateMode = false;
            return View("ChangeState");
        }
        public async Task<IActionResult> Activate(string id)
        {
            await _administrationService.ToogleState(id);
            ViewBag.IsActivateMode = true;

            return View("ChangeState");
        }

        [HttpPost]
        public async Task<IActionResult> ToogleState(string id)
        {
            await _administrationService.ToogleState(id);

            return RedirectToAction("Index");
        }
    }
}
