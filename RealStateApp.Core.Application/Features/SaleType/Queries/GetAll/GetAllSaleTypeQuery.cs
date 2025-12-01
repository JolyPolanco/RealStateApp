using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RealStateApp.Core.Application.Dtos.Improvement;
using RealStateApp.Core.Application.Dtos.SaleType;
using RealStateApp.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.SaleType.Queries.GetAll
{
    public class GetAllSaleTypeQuery : IRequest<SaleTypeResponseDto>
    {
    }

    public class GetAllImprovementQueryHandler : IRequestHandler<GetAllSaleTypeQuery, SaleTypeResponseDto>
    {
        private readonly ISaleTypeRepository _repository;
        private readonly IMapper _mapper;

        public GetAllImprovementQueryHandler(ISaleTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<SaleTypeResponseDto> Handle(GetAllSaleTypeQuery request, CancellationToken cancellationToken)
        {
            var listEntitiesQuery = _repository.GetAllQuery();

            var listEntityDtos = await listEntitiesQuery.ProjectTo<SaleTypeDto>(_mapper.ConfigurationProvider).ToListAsync();

            return new SaleTypeResponseDto
            {
                Sales = listEntityDtos
            };
        }
    }
    }

