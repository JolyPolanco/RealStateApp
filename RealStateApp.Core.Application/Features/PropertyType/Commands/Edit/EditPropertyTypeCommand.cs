using MediatR;
using RealStateApp.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.PropertyType.Commands.Edit
{
    public class EditPropertyTypeCommand : IRequest <Unit>
    {
        public required string Name { get; set; }
        public required string Description { get; set; }

        public required int Id { get; set; }

    }


    public class EditPropertyTypeCommandHandler : IRequestHandler<EditPropertyTypeCommand, Unit>
    {
        private IPropertyTypeRepository _repository;

        public EditPropertyTypeCommandHandler(IPropertyTypeRepository repository)
        {
            _repository = repository;
        }
        public async Task<Unit> Handle(EditPropertyTypeCommand request, CancellationToken cancellationToken)
        {

            Domain.Entities.PropertyType? getEntity = await _repository.GetByIdAsync(request.Id);

            if (getEntity == null)
                throw new ArgumentNullException("Tipo de propiedad no encontrada con ese Id");

            Domain.Entities.PropertyType? entity = new()
            {
                Description = request.Description,
                Name = request.Name,
                Id = request.Id
            };
            await _repository.UpdateAsync(request.Id, entity);
            return Unit.Value;
        }
    }
}
