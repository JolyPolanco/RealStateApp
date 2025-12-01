using AutoMapper;
using MediatR;
using RealStateApp.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.PropertyType.Commands.Delete
{
    public class DeletePropertyTypeCommand : IRequest<Unit>
    {

        public required int Id { get; set; }

    }
    public class DeletePropertyTypeCommandHandler : IRequestHandler<DeletePropertyTypeCommand, Unit>
    {
        private IPropertyRepository _repository;
        private readonly IMapper _mapper;

        public DeletePropertyTypeCommandHandler(IPropertyRepository repository, IMapper mapper)
        {
            _repository=repository;
            _mapper = mapper;

        }
        public async Task<Unit> Handle(DeletePropertyTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);

            if (entity == null)
                throw new ArgumentNullException("Tipo de propiedad no encontrado con ese Id");

            await _repository.DeleteAsync(request.Id);
            return Unit.Value;
        }
    }
    }

