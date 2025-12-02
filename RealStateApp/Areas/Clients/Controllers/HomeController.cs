using AutoMapper;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Core.Application.Dtos.FavoriteProperty;
using RealStateApp.Core.Application.Dtos.Properties;
using RealStateApp.Core.Application.Dtos.User;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Application.ViewModels.FavoriteProperty;
using RealStateApp.Core.Application.ViewModels.Message;
using RealStateApp.Core.Application.ViewModels.offer;
using RealStateApp.Core.Application.ViewModels.Properties;
using RealStateApp.Core.Application.ViewModels.User;
using RealStateApp.Core.Domain.Common.Enums;
using RealStateApp.Infraestructure.Identity.Entities;

namespace RealStateApp.Areas.Clients.Controllers
{
    [Area("Clients")]
    [Authorize(Roles = "CLIENT")]
    public class HomeController : Controller
    {

        private readonly IHomeClienteService _homeClienteService;
        private readonly IFavoritePropertyService _favoritePropertyService;
        private readonly IOfferService _offerService;
        private readonly IMessageService messageService;
        private readonly IMapper mapper;
        private readonly IPropertyService propertyService;
        private readonly UserManager<AppUser> userManager;



        public HomeController(IHomeClienteService _homeClienteService, IFavoritePropertyService _favoritePropertyService, IMapper mapper,
            IMessageService messageService, IOfferService _offerService, UserManager<AppUser> userManager, IPropertyService propertyService)
        {

            this.mapper = mapper;
            this._favoritePropertyService = _favoritePropertyService;
            this._homeClienteService = _homeClienteService;
            this.messageService = messageService;
            this._offerService = _offerService;
            this.propertyService = propertyService;
            this.userManager = userManager;
        }




        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await userManager.GetUserAsync(User);

            var Dataproperty = await _homeClienteService.ListProperty(user!.Id);
            var DataFavorite = new CreateFavoriteViewModel() { ClientId = user.Id, PropertyId = 0, Id = 0, FavoriteId = 0, IsFavorite = false };

            var properties = mapper.Map<List<DataPropertyViewModel>>(Dataproperty);
            var vm = new DataPropertyListViewModel() { CreateFavoriteViewModel = DataFavorite, ListProperty = properties };


            return View(vm);
        }





        [HttpGet]
        public async Task<IActionResult> DetailsProperty(int propertyId)
        {


            var user = await userManager.GetUserAsync(User);

            if (propertyId <= 0)
            {

                TempData["Error"] = "Ocurrio un error al intentar ver los detalles de esta propiedad";
                return View("Index");
            }
            bool DisableButtoOffer = await _offerService.IsOfferActive(user!.Id, propertyId);
            var detailproperty = await propertyService.GetByIdAsync(propertyId);
            var details = await _homeClienteService.ListDetailProperty(propertyId);
            var ListOffer = await _offerService.GetListByIdClientAndPropertyIdAsync(user!.Id, propertyId);
            var Conversaction = await messageService.GetConversaction(propertyId, user.Id, detailproperty!.AgentId);

            var vm = new DetailsPropertyCompleteViewModel()
            {

                CreateMessageViewModel = new CreateMessageViewModel()
                {

                    Content = "",
                    Date = DateTime.Now,
                    Id = 0,
                    PropertyId = propertyId,
                    ReceiverUserId = "",
                    SenderUserId = user.Id,
                },
                CreateOfferViewModel = new CreateOfferViewModel()
                {

                    ClientId = user.Id,
                    Amount = 0,
                    Status = OfferStatus.Pending,
                    Date = DateTime.Now,
                    DisableButton = false,
                    Id = 0,
                    PropertyId = 0
                },
                DetailsPropertyViewModel = mapper.Map<DetailsPropertyViewModel>(details),
                DataListOffer = mapper.Map<List<DataOfferViewModel>>(ListOffer),
                DataConversactionList = mapper.Map<List<DataConversactionViewModel>>(Conversaction),
                DesableButtonOffer = DisableButtoOffer,
            };


            return View(vm);
        }




        [HttpGet]
        public async Task<IActionResult> FilterAjax(string? code, string? propertyType, decimal? priceMIN, decimal? priceMax, int? bedrooms, int? bathrooms)
        {
            var user = await userManager.GetUserAsync(User);


            if (!string.IsNullOrEmpty(code))
            {
                try
                {
                    var dto = await _homeClienteService.FilterByCode(code, user!.Id);

                    var map = new List<DataPropertyViewModel>();

                    if (dto != null)
                    {
                        map = mapper.Map<List<DataPropertyViewModel>>(new List<DataPropertyDto> { dto });
                    }

                    return PartialView("_PropertyList", new PropertyListViewModel() { DataProperties = map });
                }
                catch
                {

                    return PartialView("_PropertyList", new PropertyListViewModel());
                }
            }


            var data = await _homeClienteService.FilterMultiple(
                propertyType, priceMIN, priceMax, bedrooms, bathrooms, user!.Id);


            var MapData = mapper.Map<List<DataPropertyViewModel>>(data);

            return PartialView("_PropertyList", new PropertyListViewModel() { DataProperties = MapData });
        }








        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsFavorite(CreateFavoriteViewModel vm)
        {

            var user = await userManager.GetUserAsync(User);

            vm.ClientId = user!.Id;

            if (!ModelState.IsValid)
            {

                TempData["Error"] = "Ocurrio un error al intentar marcar como favorita esta propiedad";
                return Redirect(vm.ReturnUrl!);
            }


            var Favorite = mapper.Map<CreateFavoritePropertyDto>(vm);
            await _favoritePropertyService.AddAsync(Favorite);
            TempData["Succes"] = "Propiedad marcada como favorita exitosamente";
            return Redirect(vm.ReturnUrl!);
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsNoFavorite(int id, string ReturnUrl)
        {

            if (id <= 0)
            {

                TempData["Error"] = "Ocurrio un error al intentar marcar esta propiedad como no favorita";
                return Redirect(ReturnUrl);

            }


            await _favoritePropertyService.DeleteAsync(id);
            TempData["Succes"] = "Propiedad desmarcada de favorita exitosamente";
            return Redirect(ReturnUrl);
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


            var ListAgents =  agents.Select(s => new AgentDataViewModel()
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
        public async Task<IActionResult> FilterAgents(string name)
        {


            var agents = await _homeClienteService.GetAgentByName(name);

            if (agents == null || agents.Count < 0)
            {

                TempData["Error"] = "Ocurrio un error al obtener el agente";
                return View("Agents");
            }



            var ListAgents = agents.Select(s => new AgentDataViewModel()
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                UrlImage = s.UrlImage
            }).ToList();


            var vm = new AgentDataListViewModel() { Agents = ListAgents };

            return View("Agents", vm);
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

            return PartialView("_AgentList", vm);
        }






        [HttpGet]
        public async Task<IActionResult> AgentProperty(string agentId)
        {

            var user = await userManager.GetUserAsync(User);


            var DataFavorite = new CreateFavoriteViewModel() { ClientId = user!.Id, PropertyId = 0, Id = 0, FavoriteId = 0, IsFavorite = false };

            var Properties = await _homeClienteService.ListPropertyAgentAsync(agentId,user!.Id);

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
        public async Task<IActionResult> FavoriteProperty()
        {


            var user = await userManager.GetUserAsync(User);

            var PropertyFavorite = await _favoritePropertyService.GetListPropertyFavoriteAsync(user!.Id);

            if (PropertyFavorite == null || PropertyFavorite.Count < 0)
            {

                TempData["Error"] = "Ocurrio un error al carga las propiedades favoritas";
                return View();
            }

            var ListAgents = mapper.Map<List<DataPropertyViewModel>>(PropertyFavorite);

            var vm = new PropertyListViewModel() { DataProperties = ListAgents };

            return View(vm);
        }


    }

}
