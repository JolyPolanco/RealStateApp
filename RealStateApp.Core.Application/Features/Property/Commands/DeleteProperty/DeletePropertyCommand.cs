using MediatR;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Domain.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace RealStateApp.Core.Application.Features.Property.Commands.DeleteProperty
{
    /// <summary>
    /// Parameters for deleting a property
    /// </summary>
    public class DeletePropertyCommand : IRequest<Unit>
    {
        [SwaggerParameter(Description = "Property code (6 characters)")]
        public required string Code { get; set; }
    }

    public class DeletePropertyCommandHandler : IRequestHandler<DeletePropertyCommand, Unit>
    {
        private readonly IPropertyRepository _propertyRepository;

        public DeletePropertyCommandHandler(IPropertyRepository propertyRepository)
        {
            _propertyRepository = propertyRepository;
        }

        public async Task<Unit> Handle(DeletePropertyCommand request, CancellationToken cancellationToken)
        {
            // Buscar propiedad por código
            var existingProperty = await _propertyRepository.GetByCode(request.Code);

            if (existingProperty == null)
                throw new ApiException("La propiedad no existe con ese código", (int)HttpStatusCode.NotFound);

            await _propertyRepository.DeleteAsync(existingProperty.Id);
            return Unit.Value;
        }
    }
}
