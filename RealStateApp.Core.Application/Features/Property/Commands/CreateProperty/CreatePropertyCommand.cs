using MediatR;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Domain.Entities;
using RealStateApp.Core.Domain.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace RealStateApp.Core.Application.Features.Property.Commands.CreateProperty
{
    /// <summary>
    /// Parameters for creating a property
    /// </summary>
    public class CreatePropertyCommand : IRequest<int>
    {
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

        [SwaggerParameter(Description = "Agent ID")]
        public required string AgentId { get; set; }

        [SwaggerParameter(Description = "List of improvement IDs")]
        public List<int> ImprovementIds { get; set; } = new List<int>();
    }

    public class CreatePropertyCommandHandler : IRequestHandler<CreatePropertyCommand, int>
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IPropertyTypeRepository _propertyTypeRepository;
        private readonly ISaleTypeRepository _saleTypeRepository;

        public CreatePropertyCommandHandler(
            IPropertyRepository propertyRepository,
            IPropertyTypeRepository propertyTypeRepository,
            ISaleTypeRepository saleTypeRepository)
        {
            _propertyRepository = propertyRepository;
            _propertyTypeRepository = propertyTypeRepository;
            _saleTypeRepository = saleTypeRepository;
        }

        public async Task<int> Handle(CreatePropertyCommand request, CancellationToken cancellationToken)
        {
            // Validar que exista el PropertyType
            var propertyType = await _propertyTypeRepository.GetByIdAsync(request.PropertyTypeId);
            if (propertyType == null)
                throw new ApiException("El tipo de propiedad no existe", (int)HttpStatusCode.BadRequest);

            // Validar que exista el SaleType
            var saleType = await _saleTypeRepository.GetByIdAsync(request.SaleTypeId);
            if (saleType == null)
                throw new ApiException("El tipo de venta no existe", (int)HttpStatusCode.BadRequest);

            // Generar código único de 6 caracteres
            string code = await GenerateUniqueCodeAsync();

            Domain.Entities.Property entity = new()
            {
                Id = 0,
                Code = code,
                Price = request.Price,
                SizeInMeters = request.SizeInMeters,
                Bedrooms = request.Bedrooms,
                Bathrooms = request.Bathrooms,
                Description = request.Description,
                PropertyTypeId = request.PropertyTypeId,
                SaleTypeId = request.SaleTypeId,
                AgentId = request.AgentId,
                Status = Domain.Common.Enums.PropertyStatus.Available
            };

            entity = await _propertyRepository.AddAsync(entity);

            if (entity == null)
                throw new ApiException("Error creating property", (int)HttpStatusCode.InternalServerError);

            // Agregar mejoras si se especificaron
            if (request.ImprovementIds.Any())
            {
                foreach (var improvementId in request.ImprovementIds)
                {
                    var propertyImprovement = new PropertyImprovement
                    {
                        PropertyId = entity.Id,
                        ImprovementId = improvementId
                    };
                    entity.PropertyImprovements.Add(propertyImprovement);
                }
                await _propertyRepository.UpdateAsync(entity.Id, entity);
            }

            return entity.Id;
        }

        private async Task<string> GenerateUniqueCodeAsync()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            string code;

            do
            {
                code = new string(Enumerable.Repeat(chars, 6)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                var existingProperty = await _propertyRepository.GetByCode(code);
                
                if (existingProperty == null)
                    break;

            } while (true);

            return code;
        }
    }
}
