using MediatR;
using RealStateApp.Core.Application.Features.SaleType.Commands.CreateSaleType;
using RealStateApp.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.Property.Commands.CreateProperty
{
    public class CreatePropertyCommand: IRequest<int>
    {
    }

    public class CreatePropertyCommandHandler : IRequestHandler<CreateSaleTypeCommand, int>
    {

        private readonly ISaleTypeRepository _saleTypeRepository;
        public CreatePropertyCommandHandler(ISaleTypeRepository saleTypeRepository)
        {
            _saleTypeRepository = saleTypeRepository;
        }
        public async Task<int> Handle(CreateSaleTypeCommand request, CancellationToken cancellationToken)
        {
            Domain.Entities.SaleType entity = new()
            {
                Id = 0,
                Description = request.Description,
                Name = request.Name,
            };
            entity = await _saleTypeRepository.AddAsync(entity);
            return entity != null ? entity.Id : 0;
        }
    }
}
