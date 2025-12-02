using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Core.Application.Dtos.Message;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Application.Services;
using RealStateApp.Core.Application.ViewModels.FavoriteProperty;
using RealStateApp.Core.Application.ViewModels.Message;
using RealStateApp.Core.Application.ViewModels.Properties;
using RealStateApp.Infraestructure.Identity.Entities;

namespace RealStateApp.Areas.Clients.Controllers
{

    [Area("Clients")]
    [Authorize(Roles = "CLIENT")]
    public class MenssageController : Controller
    {

        private readonly IMessageService _messageService;
        private readonly IHomeClienteService homeClienteService;
        private readonly UserManager<AppUser> userManager;
        private readonly IMapper _mapper;



        public MenssageController(IMessageService messageService, IMapper mapper, IHomeClienteService homeClienteService, UserManager<AppUser> userManager)
        {
        
            this.homeClienteService = homeClienteService;
            this.userManager = userManager;
            this._messageService = messageService;
            this._mapper = mapper;
        
 
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMensagge(CreateMessageViewModel vm)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(vm.Content))
            {
                return Json(new
                {
                    success = false,
                    message = "Debes escribir un mensaje válido antes de enviarlo."
                });
            }

            var dto = _mapper.Map<CreateMessageDto>(vm);
            await _messageService.AddAsync(dto);

            return Json(new
            {
                success = true,
                redirectUrl = Url.Action("DetailsProperty", "Home",
                                new { area = "Clients", propertyId = vm.PropertyId })
            });
        }






        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PartialMessages(int propertyId, string agentId)
        {
            var user = await userManager.GetUserAsync(User);

            var messages = await _messageService.GetConversaction(propertyId, user!.Id, agentId);

            ViewData["CurrentUserId"] = user.Id;

            return PartialView("NewMessagePartial", messages);
        }








    }
}
