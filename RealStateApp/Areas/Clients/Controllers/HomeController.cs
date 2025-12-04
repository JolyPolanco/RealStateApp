using AutoMapper;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Core.Application.Dtos.FavoriteProperty;
using RealStateApp.Core.Application.Dtos.Properties;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Application.ViewModels.FavoriteProperty;
using RealStateApp.Core.Application.ViewModels.Message;
using RealStateApp.Core.Application.ViewModels.offer;
using RealStateApp.Core.Application.ViewModels.Properties;
using RealStateApp.Core.Domain.Common.Enums;
using RealStateApp.Infraestructure.Identity.Entities;
using RealStateApp.Core.Domain.Interfaces;

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
        private readonly IPropertyRepository _propertyRepository;
        private readonly IMessageRepository _messageRepository;



        public HomeController(IHomeClienteService _homeClienteService, IFavoritePropertyService _favoritePropertyService, IMapper mapper,
            IMessageService messageService, IOfferService _offerService, UserManager<AppUser> userManager, IPropertyService propertyService,
            IPropertyRepository propertyRepository, IMessageRepository messageRepository)
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
            bool DisableButtoOffer = await _offerService.IsOfferActive(user!.Id,propertyId);
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

                    return PartialView("_PropertyList", new PropertyListViewModel() { DataProperties = map});
                }
                catch
                {

                    return PartialView("_PropertyList", new PropertyListViewModel());
                }
            }


            var data = await _homeClienteService.FilterMultiple(
                propertyType, priceMIN, priceMax, bedrooms, bathrooms, user!.Id);


            var MapData = mapper.Map<List<DataPropertyViewModel>>(data);

            return PartialView("_PropertyList", new PropertyListViewModel() {  DataProperties = MapData});
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
                return RedirectToAction("Index");
            }


            var Favorite = mapper.Map<CreateFavoritePropertyDto>(vm);
            await _favoritePropertyService.AddAsync(Favorite);
            TempData["Succes"] = "Propiedad marcada como favorita exitosamente";
            return RedirectToAction("Index");
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsNoFavorite(int id)
        {

            if (id <= 0)
            {

                TempData["Error"] = "Ocurrio un error al intentar marcar esta propiedad como no favorita";
                return RedirectToAction("Index");

            }


            await _favoritePropertyService.DeleteAsync(id);
            TempData["Succes"] = "Propiedad desmarcada de favorita exitosamente";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Chat(int id)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Login", new { area = "" });
            }

            var property = await _propertyRepository.GetByIdAsync(id);
            if (property == null)
            {
                return NotFound();
            }

            var agent = await userManager.FindByIdAsync(property.AgentId);
            if (agent == null)
            {
                return NotFound();
            }

            var messages = await _messageRepository.GetMessagesByPropertyAndClient(id, user.Id);

            ViewBag.Property = property;
            ViewBag.Agent = agent;
            ViewBag.Messages = messages.OrderBy(m => m.Date).ToList();

            return View();
        }


    }

}
