using RealStateApp.Core.Domain.Common.Enums;

namespace RealStateApp.Core.Domain.Entities
{
    public class Property
    {
        public int Id { get; set; }
        public required string Code { get; set; }
        public decimal Price { get; set; }
        public double SizeInMeters { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public required string Description { get; set; }
        public PropertyStatus Status { get; set; } = PropertyStatus.Available;

        public int PropertyTypeId { get; set; }
        public int SaleTypeId { get; set; }
        public required string AgentId { get; set; }

        public PropertyType PropertyType { get; set; } = null!;
        public SaleType SaleType { get; set; } = null!;

        public ICollection<PropertyPhoto> Photos { get; set; } = new List<PropertyPhoto>();
        public ICollection<PropertyImprovement> PropertyImprovements { get; set; } = new List<PropertyImprovement>();
        public ICollection<Offer> Offers { get; set; } = new List<Offer>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public ICollection<FavoriteProperty> FavoriteProperties { get; set; } = new List<FavoriteProperty>();
    }
}
