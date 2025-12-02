using Microsoft.AspNetCore.Mvc;

namespace RealStateWebApi.Controllers.v1
{
    public class ImprovementsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
