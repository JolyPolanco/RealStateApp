using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Core.Application.Dtos.Offer;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Application.ViewModels.offer;

namespace RealStateApp.Areas.Clients.Controllers
{
    [Area("Clients")]
    [Authorize(Roles = "CLIENT")]
    public class OfferController : Controller
    {

        private readonly IOfferService offerService;
        private readonly IHomeClienteService homeClienteService;
        private readonly IMapper _mapper;




        public OfferController(IOfferService offerService, IMapper _mapper, IHomeClienteService homeClienteService)
        {
             this.offerService = offerService;
             this.homeClienteService = homeClienteService;
             this._mapper = _mapper;
        
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOffer(CreateOfferViewModel vm)
        {
           
            if (!vm.Amount.HasValue || vm.Amount.Value < 0.90m)
            {
              
                return Json(new
                {
                    success = false,
                    message = "Debes indicar un monto válido para la oferta"
                });
            }

         
            var entity = _mapper.Map<CreateOfferDto>(vm);
            entity.Amount = vm.Amount.Value;
            entity.PropertyId = vm.Id;
            await offerService.AddAsync(entity);

           
            return Json(new
            {
                success = true,
                redirectUrl = Url.Action("DetailsProperty", "Home", new { area = "Clients", propertyId = vm.Id })
            });
        }

    }
}
