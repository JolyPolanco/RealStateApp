using RealStateApp.Core.Domain.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Dtos.Offer
{
    public class DataListOfferDto
    {

        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public OfferStatus Status { get; set; } = OfferStatus.Pending;


    }
}
