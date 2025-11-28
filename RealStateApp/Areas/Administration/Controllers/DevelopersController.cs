using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Core.Application.Dtos.User;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Application.ViewModels.User;

namespace RealStateApp.Areas.Administration.Controllers
{
    [Area("Administration")]
    public class DevelopersController : Controller
    {
        private readonly IAdministrationService _administrationService;
        private readonly IMapper _mapper;
        private readonly IAccountServiceForWebApp _accountService;

        public DevelopersController(IAdministrationService administrationService, IMapper mapper, IAccountServiceForWebApp accountService)
        {
            _administrationService = administrationService;
            _mapper = mapper;
            _accountService= accountService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var developers = await _administrationService.GetDevelopers();
            var vms = _mapper.Map<List<UserViewModel>>(developers);
            return View(vms);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("Save", new SaveBasicUserViewModel() { Dni="", Email="", FirstName="", LastName="", Role="", UserName=""});
        }

        [HttpPost]
        public async Task<IActionResult> Create(SaveBasicUserViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("Save", vm);

            var dto = _mapper.Map<SaveUserDto>(vm);
            dto.Roles.Add("DEVELOPER");

            var origin = Request.Headers.Origin;
            await _accountService.RegisterAsync(dto, origin);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _accountService.GetById(id);
            var vm = _mapper.Map<SaveBasicUserViewModel>(user);

            return View("Save", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SaveBasicUserViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("Save", vm);
            var dto = _mapper.Map<SaveUserDto>(vm);
            await _accountService.EditUser(dto);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ToggleState(string id)
        {
            var user = await _accountService.GetById(id);

            ViewBag.IsActivateMode = !user.IsActive;
            return View("ToggleState", id);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleState(string id, bool confirm = true)
        {
            if (confirm)
                await _administrationService.ToogleState(id);

            return RedirectToAction("Index");
        }
    }
}
