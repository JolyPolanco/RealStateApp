using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RealStateApp.Core.Application.Dtos.Improvement;
using RealStateApp.Core.Application.Dtos.Properties;
using RealStateApp.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.PropertyType.Queries.GetAll
{
    public class GetAllPropertyTypeQuery :  IRequest<PropertyTypeResponseDto>
    {

    }


    public class GetAllPropertyTypeQueryHandler : IRequestHandler<GetAllPropertyTypeQuery, PropertyTypeResponseDto>
    {
        private IPropertyTypeRepository _repository;
        private IMapper _mapper;

        public GetAllPropertyTypeQueryHandler(IPropertyTypeRepository repository, IMapper mapper)
        {
            _repository= repository;
            _mapper= mapper;
        }
        public  async Task<PropertyTypeResponseDto> Handle(GetAllPropertyTypeQuery request, CancellationToken cancellationToken)
        {
            var listEntitiesQuery = _repository.GetAllQuery();

            var listEntityDtos = await listEntitiesQuery.ProjectTo<PropertyTypeDto>(_mapper.ConfigurationProvider).ToListAsync();

            return new PropertyTypeResponseDto
            {
                PropertyTypes = listEntityDtos
            };
        }
    }
}
