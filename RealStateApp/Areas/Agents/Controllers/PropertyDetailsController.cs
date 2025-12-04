using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Infraestructure.Identity.Entities;
using RealStateApp.Core.Domain.Interfaces;
using AutoMapper;

namespace RealStateApp.Areas.Agents.Controllers
{
    [Area("Agents")]
    [Authorize(Roles = "AGENT")]
    public class PropertyDetailsController : Controller
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IOfferRepository _offerRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public PropertyDetailsController(
            IPropertyRepository propertyRepository,
            IMessageRepository messageRepository,
            IOfferRepository offerRepository,
            IMapper mapper,
            UserManager<AppUser> userManager)
        {
            _propertyRepository = propertyRepository;
            _messageRepository = messageRepository;
            _offerRepository = offerRepository;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Login", new { area = "" });
            }

            var property = await _propertyRepository.GetByIdAsync(id);
            if (property == null || property.AgentId != user.Id)
            {
                return RedirectToAction("AccessDenied", "Login", new { area = "" });
            }

            var messages = await _messageRepository.GetMessagesByPropertyId(id);
            var clientsWithMessages = messages
                .Where(m => m.SenderUserId != user.Id)
                .Select(m => m.SenderUserId)
                .Distinct()
                .ToList();

            var offers = await _offerRepository.GetOffersByPropertyId(id);
            var clientsWithOffers = offers
                .Select(o => o.ClientId)
                .Distinct()
                .ToList();

            var clientsWithMessagesInfo = new List<AppUser>();
            foreach (var clientId in clientsWithMessages)
            {
                var client = await _userManager.FindByIdAsync(clientId);
                if (client != null) clientsWithMessagesInfo.Add(client);
            }

            var clientsWithOffersInfo = new List<AppUser>();
            foreach (var clientId in clientsWithOffers)
            {
                var client = await _userManager.FindByIdAsync(clientId);
                if (client != null) clientsWithOffersInfo.Add(client);
            }

            ViewBag.Property = property;
            ViewBag.ClientsWithMessages = clientsWithMessagesInfo;
            ViewBag.ClientsWithOffers = clientsWithOffersInfo;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Chat(int id, string clientId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Login", new { area = "" });
            }

            var property = await _propertyRepository.GetByIdAsync(id);
            if (property == null || property.AgentId != user.Id)
            {
                return RedirectToAction("AccessDenied", "Login", new { area = "" });
            }

            var client = await _userManager.FindByIdAsync(clientId);
            if (client == null)
            {
                return NotFound();
            }

            var messages = await _messageRepository.GetMessagesByPropertyAndClient(id, clientId);

            ViewBag.Property = property;
            ViewBag.Client = client;
            ViewBag.Messages = messages.OrderBy(m => m.Date).ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(int propertyId, string clientId, string content)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Login", new { area = "" });
            }

            var property = await _propertyRepository.GetByIdAsync(propertyId);
            if (property == null || property.AgentId != user.Id)
            {
                return RedirectToAction("AccessDenied", "Login", new { area = "" });
            }

            var message = new Core.Domain.Entities.Message
            {
                PropertyId = propertyId,
                SenderUserId = user.Id,
                ReceiverUserId = clientId,
                Content = content,
                Date = DateTime.Now
            };

            await _messageRepository.AddAsync(message);

            return RedirectToAction("Chat", new { id = propertyId, clientId = clientId });
        }

        [HttpGet]
        public async Task<IActionResult> Offers(int id, string clientId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Login", new { area = "" });
            }

            var property = await _propertyRepository.GetByIdAsync(id);
            if (property == null || property.AgentId != user.Id)
            {
                return RedirectToAction("AccessDenied", "Login", new { area = "" });
            }

            var client = await _userManager.FindByIdAsync(clientId);
            if (client == null)
            {
                return NotFound();
            }

            var offers = await _offerRepository.GetOffersByPropertyAndClient(id, clientId);

            ViewBag.Property = property;
            ViewBag.Client = client;
            ViewBag.Offers = offers.OrderByDescending(o => o.Date).ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptOffer(int offerId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Login", new { area = "" });
            }

            var offer = await _offerRepository.GetByIdAsync(offerId);
            if (offer == null)
            {
                return NotFound();
            }

            var property = await _propertyRepository.GetByIdAsync(offer.PropertyId);
            if (property == null || property.AgentId != user.Id)
            {
                return RedirectToAction("AccessDenied", "Login", new { area = "" });
            }

            // Aceptar la oferta
            offer.Status = Core.Domain.Common.Enums.OfferStatus.Accepted;
            await _offerRepository.UpdateAsync(offerId, offer);

            // Rechazar todas las demás ofertas pendientes de esta propiedad
            var allOffers = await _offerRepository.GetOffersByPropertyId(property.Id);
            foreach (var otherOffer in allOffers.Where(o => o.Id != offerId && o.Status == Core.Domain.Common.Enums.OfferStatus.Pending))
            {
                otherOffer.Status = Core.Domain.Common.Enums.OfferStatus.Rejected;
                await _offerRepository.UpdateAsync(otherOffer.Id, otherOffer);
            }

            // Marcar la propiedad como vendida
            property.Status = Core.Domain.Common.Enums.PropertyStatus.Sold;
            await _propertyRepository.UpdateAsync(property.Id, property);

            TempData["SuccessMessage"] = "Oferta aceptada exitosamente. La propiedad ha sido marcada como vendida.";
            return RedirectToAction("Offers", new { id = property.Id, clientId = offer.ClientId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectOffer(int offerId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Login", new { area = "" });
            }

            var offer = await _offerRepository.GetByIdAsync(offerId);
            if (offer == null)
            {
                return NotFound();
            }

            var property = await _propertyRepository.GetByIdAsync(offer.PropertyId);
            if (property == null || property.AgentId != user.Id)
            {
                return RedirectToAction("AccessDenied", "Login", new { area = "" });
            }

            // Rechazar la oferta
            offer.Status = Core.Domain.Common.Enums.OfferStatus.Rejected;
            await _offerRepository.UpdateAsync(offerId, offer);

            TempData["SuccessMessage"] = "Oferta rechazada exitosamente.";
            return RedirectToAction("Offers", new { id = property.Id, clientId = offer.ClientId });
        }
    }
}
