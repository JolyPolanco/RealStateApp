using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Core.Application.Dtos.User;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Application.ViewModels.User;
using RealStateApp.Helpers;

namespace RealStateApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IAccountServiceForWebApp _accountService;
        private readonly IMapper _mapper;

        public UserController(IAccountServiceForWebApp accountService, IMapper mapper)
        {
            _accountService = accountService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Register()
        {
            // Si ya está autenticado, redirigir
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel vm, IFormFile? Photo)
        {
            // Si ya está autenticado, redirigir
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            // Validaciones adicionales
            if (vm.UserType != "CLIENT" && vm.UserType != "AGENT")
            {
                vm.HasError = true;
                vm.Error = "Debe seleccionar un tipo de usuario válido (Cliente o Agente)";
                return View(vm);
            }

            // Validar que hay foto
            if (Photo == null || Photo.Length == 0)
            {
                vm.HasError = true;
                vm.Error = "La foto de perfil es requerida";
                return View(vm);
            }

            // Guardar la foto temporalmente (sin ID porque el usuario aún no existe)
            string photoPath = FileHelper.UploadWithoutId(Photo, "Users");

            // Crear el DTO para el servicio
            var dto = _mapper.Map<SaveUserDto>(vm);
            dto.Photo = photoPath;
            dto.Roles = new List<string> { vm.UserType };

            string origin = $"{Request.Scheme}://{Request.Host}";
            RegisterUserResponseDto result = await _accountService.RegisterAsync(dto, origin);

            if (result.HasError)
            {
                vm.HasError = true;
                vm.Error = string.Join(", ", result.Errors ?? new List<string>());
                
                // Eliminar la foto si hubo error
                if (!string.IsNullOrEmpty(photoPath))
                {
                    FileHelper.DeleteFile(photoPath);
                }
                
                return View(vm);
            }

            // Mensaje de éxito según el tipo de usuario
            if (vm.UserType == "CLIENT")
            {
                TempData["Success"] = "¡Tu cuenta se ha creado correctamente! Revisa tu correo electrónico para activar tu cuenta y poder iniciar sesión.";
            }
            else // AGENT
            {
                TempData["Success"] = "¡Tu cuenta se ha creado correctamente! Un administrador revisará tu solicitud y te notificaremos cuando puedas iniciar sesión.";
            }

            return RedirectToAction("Index", "Login");
        }
    }
}
