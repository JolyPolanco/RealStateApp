using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Application.ViewModels.User;
using RealStateApp.Helpers;
using RealStateApp.Infraestructure.Identity.Entities;

namespace RealStateApp.Areas.Agents.Controllers
{
    [Area("Agents")]
    [Authorize(Roles = "AGENT")]
    public class ProfileController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public ProfileController(UserManager<AppUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Login", new { area = "" });
            }

            var viewModel = new EditAgentProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber ?? "",
                Photo = user.Photo
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(EditAgentProfileViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Login", new { area = "" });
            }

            user.FirstName = vm.FirstName;
            user.LastName = vm.LastName;
            user.PhoneNumber = vm.PhoneNumber;

            if (vm.PhotoFile != null)
            {
                user.Photo = FileHelper.UploadWithoutId(vm.PhotoFile, "Agents", user.Photo ?? "", true);
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Perfil actualizado correctamente";
                return RedirectToAction("Index");
            }

            vm.HasError = true;
            vm.Error = "Ocurrió un error al actualizar el perfil";
            return View(vm);
        }
    }
}
