using Microsoft.AspNetCore.Mvc;

namespace RealStateApp.Areas.Clients.Controllers
{
    [Area("Clients")]

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
