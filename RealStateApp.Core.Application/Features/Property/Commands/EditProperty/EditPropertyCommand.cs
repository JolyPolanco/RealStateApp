using MediatR;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Domain.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace RealStateApp.Core.Application.Features.Property.Commands.EditProperty
{
    /// <summary>
    /// Parameters for editing a property
    /// </summary>
    public class EditPropertyCommand : IRequest<Unit>
    {
        [SwaggerParameter(Description = "Property ID")]
        public int Id { get; set; }

        [SwaggerParameter(Description = "Price of the property")]
        public decimal Price { get; set; }

        [SwaggerParameter(Description = "Size in square meters")]
        public double SizeInMeters { get; set; }

        [SwaggerParameter(Description = "Number of bedrooms")]
        public int Bedrooms { get; set; }

        [SwaggerParameter(Description = "Number of bathrooms")]
        public int Bathrooms { get; set; }

        [SwaggerParameter(Description = "Property description")]
        public required string Description { get; set; }

        [SwaggerParameter(Description = "Property type ID")]
        public int PropertyTypeId { get; set; }

        [SwaggerParameter(Description = "Sale type ID")]
        public int SaleTypeId { get; set; }

        [SwaggerParameter(Description = "Property status")]
        public int Status { get; set; }

        [SwaggerParameter(Description = "List of improvement IDs")]
        public List<int> ImprovementIds { get; set; } = new List<int>();
    }

    public class EditPropertyCommandHandler : IRequestHandler<EditPropertyCommand, Unit>
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IPropertyTypeRepository _propertyTypeRepository;
        private readonly ISaleTypeRepository _saleTypeRepository;
        private readonly IPropertyImprovementRepository _propertyImprovementRepository;

        public EditPropertyCommandHandler(
            IPropertyRepository propertyRepository,
            IPropertyTypeRepository propertyTypeRepository,
            ISaleTypeRepository saleTypeRepository,
            IPropertyImprovementRepository propertyImprovementRepository)
        {
            _propertyRepository = propertyRepository;
            _propertyTypeRepository = propertyTypeRepository;
            _saleTypeRepository = saleTypeRepository;
            _propertyImprovementRepository = propertyImprovementRepository;
        }

        public async Task<Unit> Handle(EditPropertyCommand request, CancellationToken cancellationToken)
        {
            // Buscar propiedad por ID
            var existingProperty = await _propertyRepository.GetByIdAsync(request.Id);

            if (existingProperty == null)
                throw new ApiException("La propiedad no existe con ese ID", (int)HttpStatusCode.NotFound);

            // Validar que exista el PropertyType
            var propertyType = await _propertyTypeRepository.GetByIdAsync(request.PropertyTypeId);
            if (propertyType == null)
                throw new ApiException("El tipo de propiedad no existe", (int)HttpStatusCode.BadRequest);

            // Validar que exista el SaleType
            var saleType = await _saleTypeRepository.GetByIdAsync(request.SaleTypeId);
            if (saleType == null)
                throw new ApiException("El tipo de venta no existe", (int)HttpStatusCode.BadRequest);

            // Actualizar propiedades
            existingProperty.Price = request.Price;
            existingProperty.SizeInMeters = request.SizeInMeters;
            existingProperty.Bedrooms = request.Bedrooms;
            existingProperty.Bathrooms = request.Bathrooms;
            existingProperty.Description = request.Description;
            existingProperty.PropertyTypeId = request.PropertyTypeId;
            existingProperty.SaleTypeId = request.SaleTypeId;
            existingProperty.Status = (Domain.Common.Enums.PropertyStatus)request.Status;

            // Primero actualizar la propiedad sin tocar las mejoras
            await _propertyRepository.UpdateAsync(existingProperty.Id, existingProperty);

            // Eliminar todas las mejoras existentes
            await _propertyImprovementRepository.DeleteByPropertyId(existingProperty.Id);

            // Agregar nuevas mejoras
            foreach (var improvementId in request.ImprovementIds)
            {
                var propertyImprovement = new Domain.Entities.PropertyImprovement
                {
                    PropertyId = existingProperty.Id,
                    ImprovementId = improvementId
                };
                await _propertyImprovementRepository.AddAsync(propertyImprovement);
            }

            return Unit.Value;
        }
    }
}
