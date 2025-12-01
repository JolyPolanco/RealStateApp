


using RealStateApp.Core.Domain.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace RealStateApp.Core.Application.ViewModels.offer
{
    public class CreateOfferViewModel
    {

        public int Id { get; set; }
        [Required(ErrorMessage = "Debes indicar un monto para realizar la oferta")]
        [Range(0.90, double.MaxValue, ErrorMessage = "Debes un monto valido")]
        public decimal? Amount { get; set; }
        public DateTime Date { get; set; }
        public OfferStatus Status { get; set; } = OfferStatus.Pending;
        public int PropertyId { get; set; }
        public required string ClientId { get; set; }


        //property for disable btn
        public bool DisableButton { get; set; } = false;
    }
}
