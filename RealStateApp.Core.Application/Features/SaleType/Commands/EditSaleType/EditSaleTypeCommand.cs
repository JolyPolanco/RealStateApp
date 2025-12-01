using MediatR;
using RealStateApp.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.SaleType.Commands.EditSaleType
{
    public class EditSaleTypeCommand: IRequest<Unit>
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
    }

    public class EditSaleTypeCommandHandler : IRequestHandler<EditSaleTypeCommand, Unit>
    {
        private readonly ISaleTypeRepository _repository;

        public EditSaleTypeCommandHandler(ISaleTypeRepository repository)
        {
            _repository= repository;
        }
        public async Task<Unit> Handle(EditSaleTypeCommand request, CancellationToken cancellationToken)
        {

            Domain.Entities.SaleType? getEntity = await _repository.GetByIdAsync(request.Id);

            if (getEntity == null)
                throw new ArgumentNullException("Tipo de venta no encontrada con ese Id");

            Domain.Entities.SaleType? entity = new()
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
