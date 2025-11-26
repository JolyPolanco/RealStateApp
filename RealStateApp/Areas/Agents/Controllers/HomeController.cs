using Microsoft.AspNetCore.Mvc;

namespace RealStateApp.Areas.Agents.Controllers
{
    [Area("Agents")]

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
