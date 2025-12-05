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

namespace RealStateApp.Core.Application.Features.SaleType.Queries.GetAllWithInclude
{
    public class GetAllSaleTypeWithIncludeQuery : IRequest<IList<SaleTypeDto>>
    {

    }

    public class GetAllSaleTypeWithIncludeQueryHandler : IRequestHandler<GetAllSaleTypeWithIncludeQuery, IList<SaleTypeDto>>
    {
        private readonly ISaleTypeRepository _repository;
        private readonly IMapper _mapper;

        public GetAllSaleTypeWithIncludeQueryHandler(ISaleTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<IList<SaleTypeDto>> Handle(GetAllSaleTypeWithIncludeQuery request, CancellationToken cancellationToken)
        {
            var listEntitiesQuery = _repository.GetAllQueryWithInclude(new List<string> { "Properties" });

            var listEntityDtos = await listEntitiesQuery.ProjectTo<SaleTypeDto>(_mapper.ConfigurationProvider).ToListAsync();

            return listEntityDtos;
        }
    }
}
