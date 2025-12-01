using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RealStateApp.Core.Application.Dtos.Properties;
using RealStateApp.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.PropertyType.Commands.Create
{
    public class CreatePropertyTypeCommand: IRequest<PropertyTypeDto>
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
    }

   
    public class CreatePropertyTypeCommandHandler : IRequestHandler<CreatePropertyTypeCommand, PropertyTypeDto>
    {
        private IPropertyTypeRepository _repository;
        private IMapper _mapper;

        public CreatePropertyTypeCommandHandler(IPropertyTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<PropertyTypeDto> Handle(CreatePropertyTypeCommand request, CancellationToken cancellationToken)
        {

            var entity = await _repository.AddAsync(new Domain.Entities.PropertyType
            {
                Description = request.Description,
                Name = request.Name,
                Id = 0
            });

         return _mapper.Map<PropertyTypeDto>(entity);
        }
    }
}
