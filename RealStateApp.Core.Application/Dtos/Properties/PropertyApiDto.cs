using RealStateApp.Core.Domain.Common.Enums;

namespace RealStateApp.Core.Application.Dtos.Properties
{
    /// <summary>
    /// DTO para la API de propiedades según especificación del documento
    /// Incluye: Id, Código, Tipo de propiedad, Tipo de venta, Precio, 
    /// Tamaño del terreno, Habitaciones, Baños, Descripción, 
    /// Mejoras, Nombre del agente, Id del agente, Estado
    /// </summary>
    public class PropertyApiDto
    {
        public int Id { get; set; }
        public required string Code { get; set; }
        public required string PropertyType { get; set; }
        public required string SaleType { get; set; }
        public decimal Price { get; set; }
        public double SizeInMeters { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public required string Description { get; set; }
        public List<string> Improvements { get; set; } = new List<string>();
        public required string AgentName { get; set; }
        public required string AgentId { get; set; }
        public PropertyStatus Status { get; set; }
    }
}
