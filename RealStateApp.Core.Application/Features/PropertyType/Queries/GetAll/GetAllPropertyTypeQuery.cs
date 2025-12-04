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
    public class GetAllPropertyTypeQuery :  IRequest<IList<PropertyTypeDto>>
    {

    }


    public class GetAllPropertyTypeQueryHandler : IRequestHandler<GetAllPropertyTypeQuery, IList<PropertyTypeDto>>
    {
        private IPropertyTypeRepository _repository;
        private IMapper _mapper;

        public GetAllPropertyTypeQueryHandler(IPropertyTypeRepository repository, IMapper mapper)
        {
            _repository= repository;
            _mapper= mapper;
        }
        public  async Task<IList<PropertyTypeDto>> Handle(GetAllPropertyTypeQuery request, CancellationToken cancellationToken)
        {
            var listEntitiesQuery = _repository.GetAllQuery();

            var listEntityDtos = await listEntitiesQuery.ProjectTo<PropertyTypeDto>(_mapper.ConfigurationProvider).ToListAsync();

            return listEntityDtos;
        }
    }
}
