using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Core.Application.Dtos.Properties;
using RealStateApp.Core.Application.Dtos.User;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Application.ViewModels.FavoriteProperty;
using RealStateApp.Core.Application.ViewModels.Message;
using RealStateApp.Core.Application.ViewModels.offer;
using RealStateApp.Core.Application.ViewModels.Properties;
using RealStateApp.Core.Application.ViewModels.User;
using RealStateApp.Core.Domain.Common.Enums;
using RealStateApp.Core.Domain.Interfaces;
using RealStateApp.Infraestructure.Identity.Entities;
using System.Diagnostics;

namespace RealStateApp.Controllers
{
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> userManager;
        private readonly IAccountServiceForWebApp accountServiceForWebApp;
        private readonly IHomeClienteService _homeClienteService;
        private readonly IFavoritePropertyService _favoritePropertyService;
        private readonly IOfferService _offerService;
        private readonly IMessageService messageService;
        private readonly IMapper mapper;
        private readonly IPropertyService propertyService;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IMessageRepository _messageRepository;


        public HomeController(IHomeClienteService _homeClienteService, IFavoritePropertyService _favoritePropertyService, IMapper mapper,
          IMessageService messageService, IOfferService _offerService, UserManager<AppUser> userManager, IPropertyService propertyService,
          IPropertyRepository propertyRepository, IMessageRepository messageRepository, ILogger<HomeController> logger, IAccountServiceForWebApp accountServiceForWebApp)
        {

            this.mapper = mapper;
            this._favoritePropertyService = _favoritePropertyService;
            this._homeClienteService = _homeClienteService;
            this.messageService = messageService;
            this._offerService = _offerService;
            this.propertyService = propertyService;
            this.userManager = userManager;
            this._propertyRepository = propertyRepository;
            this._messageRepository = messageRepository;
            this._logger = logger;
            this.accountServiceForWebApp = accountServiceForWebApp;
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





        [HttpGet]
        public async Task<IActionResult> DataProperty()
        {
           

            var Dataproperty = await _homeClienteService.ListProperty("");
           

            var properties = mapper.Map<List<DataPropertyViewModel>>(Dataproperty);
            var vm = new DataPropertyListViewModel() { ListProperty = properties };

            return View(vm);
        }





        [HttpGet]
        public async Task<IActionResult> DetailsProperty(int propertyId)
        {


          

            if (propertyId <= 0)
            {

                TempData["Error"] = "Ocurrio un error al intentar ver los detalles de esta propiedad";
                return View("Index");
            }
      
            var detailproperty = await propertyService.GetByIdAsync(propertyId);
            var details = await _homeClienteService.ListDetailProperty(propertyId);
           

            var vm = new DetailsPropertyCompleteViewModel()
            {

                DetailsPropertyViewModel = mapper.Map<DetailsPropertyViewModel>(details),
              
            };


            return View(vm);
        }





        [HttpGet]
        public async Task<IActionResult> FilterAjax(string? code, string? propertyType, decimal? priceMIN, decimal? priceMax, int? bedrooms, int? bathrooms)
        {
            


            if (!string.IsNullOrEmpty(code))
            {
                try
                {
                    var dto = await _homeClienteService.FilterByCode(code, "");

                    var map = new List<DataPropertyViewModel>();

                    if (dto != null)
                    {
                        map = mapper.Map<List<DataPropertyViewModel>>(new List<DataPropertyDto> { dto });
                    }

                    return PartialView("_ListPropertyHome", new PropertyListViewModel() { DataProperties = map });
                }
                catch
                {

                    return PartialView("_ListPropertyHome", new PropertyListViewModel());
                }
            }


            var data = await _homeClienteService.FilterMultiple(
                propertyType, priceMIN, priceMax, bedrooms, bathrooms, "");


            var MapData = mapper.Map<List<DataPropertyViewModel>>(data);

            return PartialView("_ListPropertyHome", new PropertyListViewModel() { DataProperties = MapData });
        }







       
        
        
        [HttpGet]
        public async Task<IActionResult> Agents()
        {



            var agents = await _homeClienteService.ListAgentAsync();

            if (agents == null || agents.Count < 0)
            {

                TempData["Error"] = "Ocurrio un error al cargar los agentes";
                return View();
            }


            var ListAgents = agents.Select(s => new AgentDataViewModel()
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                UrlImage = s.UrlImage
            }).ToList();


            var vm = new AgentDataListViewModel() { Agents = ListAgents };

            return View(vm);
        }





        [HttpGet]
        public async Task<IActionResult> AgentProperty(string agentId)
        {

           


         
            var Properties = await _homeClienteService.ListPropertyAgentAsync(agentId, "");

            if (Properties == null || Properties.Count < 0)
            {

                TempData["Error"] = "Ocurrio un error al obtener la propieda del agente";
                return View("Agents");
            }

            var ListAgents = mapper.Map<List<DataPropertyViewModel>>(Properties);


            var vm = new PropertyListViewModel() { DataProperties = ListAgents };

            return View(vm);
        }





        [HttpGet]
        public async Task<IActionResult> FilterAgentsAjax(string name)
        {
            var agents = await _homeClienteService.GetAgentByName(name);

            if (agents == null || !agents.Any())
                agents = new List<AgentDataDto>();

            var ListAgents = agents.Select(s => new AgentDataViewModel()
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                UrlImage = s.UrlImage
            }).ToList();

            var vm = new AgentDataListViewModel() { Agents = ListAgents };

            return PartialView("_AgentListPartial", vm);
        }




    }
}
