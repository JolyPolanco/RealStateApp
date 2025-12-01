using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;

namespace RealStateWebApi.Controllers.v1
{
    [SwaggerTag("PropertyTypes CRUD")]

    public class PropertyTypesController : Controller
    {


        [HttpGet]

        [Consumes(MediaTypeNames.Application.Json)]
        [SwaggerOperation(
          Summary = "Listado",
            Description = "Retorna el listado"
            )]
        public IActionResult Index()
        {
            return View();
        }
    }
}
