using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace RealStateApp.Core.Application.ViewModels.Properties
{
    public class SavePropertyViewModel
    {
        public int? Id { get; set; }
        
        [Required(ErrorMessage = "El tipo de propiedad es requerido")]
        [Display(Name = "Tipo de propiedad")]
        public int PropertyTypeId { get; set; }
        
        [Required(ErrorMessage = "El tipo de venta es requerido")]
        [Display(Name = "Tipo de venta")]
        public int SaleTypeId { get; set; }
        
        [Required(ErrorMessage = "El precio es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que 0")]
        [Display(Name = "Precio (DOP)")]
        public decimal? Price { get; set; }
        
        [Required(ErrorMessage = "El tamaño es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El tamaño debe ser mayor que 0")]
        [Display(Name = "Tamaño en metros")]
        public double? SizeInMeters { get; set; }
        
        [Required(ErrorMessage = "La cantidad de habitaciones es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe tener al menos 1 habitación")]
        [Display(Name = "Habitaciones")]
        public int? Bedrooms { get; set; }
        
        [Required(ErrorMessage = "La cantidad de baños es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe tener al menos 1 baño")]
        [Display(Name = "Baños")]
        public int? Bathrooms { get; set; }
        
        [Required(ErrorMessage = "La descripción es requerida")]
        [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        [Display(Name = "Descripción")]
        public string? Description { get; set; }
        
        [Display(Name = "Mejoras")]
        public List<int> ImprovementIds { get; set; } = new List<int>();
        
        [Display(Name = "Imágenes (1-4)")]
        public List<IFormFile>? Photos { get; set; }
        
        public List<string>? ExistingPhotos { get; set; } = new List<string>();
        
        public bool HasError { get; set; }
        public string? Error { get; set; }
    }
}
