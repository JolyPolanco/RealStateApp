using RealStateApp.Core.Domain.Common.Enums;

namespace RealStateApp.Core.Domain.Entities
{
    public class Offer
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public OfferStatus Status { get; set; } = OfferStatus.Pending;

        public int PropertyId { get; set; }
        public required string ClientId { get; set; }

        public Property Property { get; set; } = null!;
    }
}
