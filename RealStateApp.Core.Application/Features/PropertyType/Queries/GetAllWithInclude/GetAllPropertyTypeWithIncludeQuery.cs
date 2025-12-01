using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RealStateApp.Core.Application.Dtos.Properties;
using RealStateApp.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.PropertyType.Queries.GetAllWithInclude
{
    public class GetAllPropertyTypeWithIncludeQuery : IRequest<PropertyTypeResponseDto>
    {

    }


    public class GetAllPropertyTypeQueryHandler : IRequestHandler<GetAllPropertyTypeWithIncludeQuery, PropertyTypeResponseDto>
    {
        private IPropertyTypeRepository _repository;
        private IMapper _mapper;

        public GetAllPropertyTypeQueryHandler(IPropertyTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<PropertyTypeResponseDto> Handle(GetAllPropertyTypeWithIncludeQuery request, CancellationToken cancellationToken)
        {
            var listEntitiesQuery = _repository.GetAllQueryWithInclude( new List<string> { "Properties" });

            var listEntityDtos = await listEntitiesQuery.ProjectTo<PropertyTypeDto>(_mapper.ConfigurationProvider).ToListAsync();

            return new PropertyTypeResponseDto
            {
                PropertyTypes = listEntityDtos
            };
        }
    }
}
