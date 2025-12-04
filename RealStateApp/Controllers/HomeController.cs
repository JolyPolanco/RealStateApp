using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Application.ViewModels.User;
using RealStateApp.Infraestructure.Identity.Entities;
using System.Diagnostics;

namespace RealStateApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> userManager;
        private readonly IAccountServiceForWebApp accountServiceForWebApp;

        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, IAccountServiceForWebApp accountServiceForWebApp)
        {
            _logger = logger;
            this.accountServiceForWebApp = accountServiceForWebApp;
            this.userManager = userManager;
        }


        public async Task<IActionResult> Index(string message = null!)
        {
            if (!string.IsNullOrEmpty(message))
                TempData["ErrorMessage"] = message;

            // Si ya hay un usuario autenticado
            if (User.Identity?.IsAuthenticated == true)
            {
                AppUser? userSession = await userManager.GetUserAsync(User);

                if (userSession != null)
                {
                    // Verificar que el usuario est� activo y confirmado
                    if (userSession.IsActive && userSession.EmailConfirmed)
                    {
                        var roles = await userManager.GetRolesAsync(userSession);
                        if (roles.Any())
                        {
                            return RedirectToHome(roles.First());
                        }
                    }

                    // Si el usuario no est� activo o no tiene email confirmado, cerrar sesi�n
                    await accountServiceForWebApp.SignOutAsync();
                }
            }

            return View(new LoginViewModel() { Password = "", UserName = "" });
        }



        public IActionResult Privacy()
        {
            return View();
        }



        private IActionResult RedirectToHome(string role)
        {
            return role.ToUpper() switch
            {
                "ADMIN" => RedirectToRoute(new { controller = "Home", action = "Index", area = "Administration" }),
                "AGENT" => RedirectToRoute(new { area = "Agents", controller = "Home", action = "Index" }),
                "CLIENT" => RedirectToRoute(new { area = "Clients", controller = "Home", action = "Index" }),
                _ => RedirectToRoute(new { controller = "Login", action = "Index" })
            };
        }

    }
}
