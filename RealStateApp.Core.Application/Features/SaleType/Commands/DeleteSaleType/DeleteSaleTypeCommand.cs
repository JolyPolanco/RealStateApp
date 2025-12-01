using MediatR;
using RealStateApp.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.SaleType.Commands.DeleteSaleType
{
    public class DeleteSaleTypeCommand : IRequest<Unit>
    {

        public required int Id { get; set; }

    }

    public class DeleteSaleTypeCommandHandler : IRequestHandler<DeleteSaleTypeCommand, Unit>
    {
        private readonly ISaleTypeRepository _repository;

        public DeleteSaleTypeCommandHandler(ISaleTypeRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteSaleTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);

            if (entity == null)
                throw new ArgumentNullException("Tipo de venta no encontrado con ese Id");

            await _repository.DeleteAsync(request.Id);
            return Unit.Value;
        }
    }

}
