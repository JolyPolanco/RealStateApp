using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RealStateApp.Core.Application.Dtos.Properties;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.PropertyType.Commands.Create
{
    public class CreatePropertyTypeCommand: IRequest<int>
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
    }

   
    public class CreatePropertyTypeCommandHandler : IRequestHandler<CreatePropertyTypeCommand, int>
    {
        private IPropertyTypeRepository _repository;

        public CreatePropertyTypeCommandHandler(IPropertyTypeRepository repository)
        {
            _repository = repository;
         
        }
        public async Task<int> Handle(CreatePropertyTypeCommand request, CancellationToken cancellationToken)
        {

            var entity = await _repository.AddAsync(new Domain.Entities.PropertyType
            {
                Description = request.Description,
                Name = request.Name,
                Id = 0
            });

            if (entity == null) throw new  ApiException("Error creating Property Type", HttpStatusCode.InternalServerError);
         return entity.Id;
        }
    }
}
