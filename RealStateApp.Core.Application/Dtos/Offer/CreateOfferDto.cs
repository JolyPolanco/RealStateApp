using RealStateApp.Core.Domain.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Dtos.Offer
{
    public class CreateOfferDto
    {

        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public OfferStatus Status { get; set; } = OfferStatus.Pending;

        public int PropertyId { get; set; }
        public required string ClientId { get; set; }


        //property for disable btn
        public bool DisableButton { get; set; } = false;
    }
}
