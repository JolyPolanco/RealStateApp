using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace RealStateApp.Core.Application.Dtos.Properties
{
    public class SavePropertyDto
    {
        public int? Id { get; set; }
        
        [Required(ErrorMessage = "El tipo de propiedad es requerido")]
        public int PropertyTypeId { get; set; }
        
        [Required(ErrorMessage = "El tipo de venta es requerido")]
        public int SaleTypeId { get; set; }
        
        [Required(ErrorMessage = "El precio es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que 0")]
        public decimal Price { get; set; }
        
        [Required(ErrorMessage = "El tamaño es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El tamaño debe ser mayor que 0")]
        public double SizeInMeters { get; set; }
        
        [Required(ErrorMessage = "La cantidad de habitaciones es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe tener al menos 1 habitación")]
        public int Bedrooms { get; set; }
        
        [Required(ErrorMessage = "La cantidad de baños es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe tener al menos 1 baño")]
        public int Bathrooms { get; set; }
        
        [Required(ErrorMessage = "La descripción es requerida")]
        [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        public string Description { get; set; } = string.Empty;
        
        public List<int> ImprovementIds { get; set; } = new List<int>();
        
        public List<string>? ExistingPhotos { get; set; } = new List<string>();
    }
}
