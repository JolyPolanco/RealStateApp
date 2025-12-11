using AutoMapper;
using MediatR;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        private IPropertyTypeRepository _repository;

        public DeletePropertyTypeCommandHandler(IPropertyTypeRepository repository)
        {
            _repository=repository;
        

        }
        public async Task<Unit> Handle(DeletePropertyTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);

            if (entity == null)
                throw new ApiException("Entity not found with this id",(int)HttpStatusCode.NotFound);

            await _repository.DeleteAsync(request.Id);
            return Unit.Value;
        }
    }
    }

