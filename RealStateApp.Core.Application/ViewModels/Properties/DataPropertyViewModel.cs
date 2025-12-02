

namespace RealStateApp.Core.Application.ViewModels.Properties
{
    public class DataPropertyViewModel
    {
        public int Id { get; set; }
        public required string Code { get; set; }
        public required decimal Price { get; set; }
        public required double SizeInMeters { get; set; }
        public required int Bedrooms { get; set; }
        public required int Bathrooms { get; set; }
        public required string Photo { get; set; }
        public required string TypeProperty { get; set; }
        public required string SaleType { get; set; }

        public bool IsFavorite { get; set; }
        public int FavoriteId { get; set; }
        public string? ClientId { get; set; }
        public string? AgentId { get; set; }


    }
}
