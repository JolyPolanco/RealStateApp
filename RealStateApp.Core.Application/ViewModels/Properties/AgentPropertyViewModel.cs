using RealStateApp.Core.Domain.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace RealStateApp.Core.Application.ViewModels.Properties
{
    public class AgentPropertyViewModel
    {
        public int Id { get; set; }
        public required string Code { get; set; }
        public required string PropertyTypeName { get; set; }
        public required string SaleTypeName { get; set; }
        public decimal Price { get; set; }
        public double SizeInMeters { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public string? MainPhoto { get; set; }
        public PropertyStatus Status { get; set; }
        public string StatusText => Status == PropertyStatus.Sold ? "Vendida" : "Disponible";
    }
}
