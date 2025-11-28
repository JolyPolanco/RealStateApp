using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Core.Application.Dtos.User;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Application.ViewModels.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealStateApp.Areas.Administration.Controllers
{
    [Area("Administration")]
    public class AdministratorsController : Controller
    {
        private readonly IAdministrationService _administrationService;
        private readonly IAccountServiceForWebApp _accountService;
        private readonly IMapper _mapper;

        public AdministratorsController(IAdministrationService administrationService,
            IAccountServiceForWebApp accountService,
            IMapper mapper)
        {
            _administrationService = administrationService;
            _accountService = accountService;
            _mapper = mapper;
        }

        // Listado de administradores
        [HttpGet("administradores")]
        public async Task<IActionResult> Index()
        {
            var administrators = await _administrationService.GetAdministrators();
            var vms = _mapper.Map<List<UserViewModel>>(administrators);

            // Pasamos el Id del usuario logueado para evitar editarse a sí mismo
            var currentUserName = User.Identity?.Name;
            var currentUser = await _accountService.GetByUserName(currentUserName??"");
            ViewBag.CurrentUserId = currentUser.Id;
            return View(vms);
        }

        // Abrir formulario para crear administrador
        [HttpGet("administradores/crear")]
        public IActionResult Create()
        {
            return View("Save", new SaveBasicUserViewModel()
            {
                Dni= "",
                Email="",
                FirstName="",
                LastName="",
                Role="",
                UserName="",
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(SaveBasicUserViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("Save", vm);

            var dto = _mapper.Map<SaveUserDto>(vm);
            dto.Roles.Add("ADMIN");
            var origin = Request.Headers.Origin.ToString();
            await _accountService.RegisterAsync(dto, origin);
            return RedirectToAction("Index");
        }

        [HttpGet("editar/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            var admin = await _accountService.GetById (id);
            if (admin == null) return NotFound();

            var vm = _mapper.Map<SaveBasicUserViewModel>(admin);
            return View("Save", vm);
        }

        // Guardar edición
        [HttpPost("editar/{id}")]
        public async Task<IActionResult> Edit(SaveBasicUserViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("Save", vm);
            var dto = _mapper.Map<SaveUserDto>(vm);
            dto.Roles.Add("ADMIN");

            await _accountService.EditUser(dto);
            return RedirectToAction("Index");
        }

        // Cambiar estado (activar/inactivar)
        [HttpGet("cambiar-estado/{id}")]
        public async Task<IActionResult> ChangeState(string id)
        {
            var admin = await _accountService.GetById(id);
            if (admin == null) return NotFound();

            ViewBag.IsActivateMode = !admin.IsActive;
            return View("ChangeState", admin.Id);
        }

        [HttpPost("toggle/{id}")]
        public async Task<IActionResult> ToggleState(string id)
        {
            await _administrationService.ToogleState(id);
            return RedirectToAction("Index");
        }
    }
}
